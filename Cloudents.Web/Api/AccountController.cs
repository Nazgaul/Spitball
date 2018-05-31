﻿using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]

    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ICommandBus _commandBus;
        private readonly IMapper _mapper;

        public AccountController(UserManager<User> userManager, ICommandBus commandBus, IMapper mapper)
        {
            _userManager = userManager;
            _commandBus = commandBus;
            _mapper = mapper;
        }

        // GET
        [HttpGet]
        [Authorize(Policy = SignInStep.PolicyAll)]

        public async Task<IActionResult> GetAsync([FromServices] IBlockChainErc20Service blockChain, CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var balance = await blockChain.GetBalanceAsync(user.PublicKey, token).ConfigureAwait(false);
            return Ok(new
            {
                user.Id,
                user.Image,
                user.Email,
                user.Name,
                token = GetToken(),
                balance
            });
        }

        private string GetToken()
        {
            // ReSharper disable once StringLiteralTypo
            const string key = "sk_test_AQGzQ2Rlj0NeiNOEdj1SlosU";
            var message = _userManager.GetUserId(User);

            var asciiEncoding = new ASCIIEncoding();
            var keyByte = asciiEncoding.GetBytes(key);
            var messageBytes = asciiEncoding.GetBytes(message);

            using (var sha256 = new HMACSHA256(keyByte))
            {
                var hashMessage = sha256.ComputeHash(messageBytes);

                var result = new StringBuilder();
                foreach (byte b in hashMessage)
                {
                    result.Append(b.ToString("X2"));
                }
                return result.ToString();
            }
        }

        [HttpGet("userName")]
        [Authorize(Policy = SignInStep.PolicyPassword)]
        public IActionResult GetUserName()
        {
            var name = _userManager.GetUserName(User);
            return Ok(new { name });
        }

        [HttpPost("userName"), ValidateModel]
        [Authorize(Policy = SignInStep.PolicyPassword)]
        public async Task<IActionResult> ChangeUserNameAsync(
            [FromBody]ChangeUserNameRequest model,
            [FromServices] IQueueProvider client,
            CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var t1 = _userManager.SetUserNameAsync(user, model.Name);
            var userId = user.Id;
            var t2 = client.InsertBackgroundMessageAsync(new TalkJsUser(userId)
            {
                Name = user.Name
            },token);
           
            try
            {
                await Task.WhenAll(t1, t2).ConfigureAwait(false);
            }
            catch (UserNameExistsException ex)
            {
                await client.InsertBackgroundMessageAsync(new TalkJsUser(userId)
                {
                    Name = _userManager.GetUserName(User)
                }, token).ConfigureAwait(false);

                return BadRequest(ex.Message);
            }

            if (t1.Result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(t1.Result.Errors);
        }

        //TODO : need to figure out what well do.
        [HttpPost("university")]
        [Authorize(Policy = SignInStep.PolicyAll)]
        public async Task<IActionResult> AssignUniversityAsync([FromBody] AssignUniversityRequest model, CancellationToken token)
        {
            var command = _mapper.Map<AssignUniversityToUserCommand>(model);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

    }
}