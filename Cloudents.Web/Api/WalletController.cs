using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Application.Command;
using Cloudents.Application.DTOs;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Query;
using Cloudents.Domain.Entities;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly UserManager<RegularUser> _userManager;
        private readonly ILogger _logger;

        public WalletController(UserManager<RegularUser> userManager, IQueryBus queryBus, ILogger logger)
        {
            _userManager = userManager;
            _queryBus = queryBus;
            _logger = logger;
        }

        // GET
        [HttpGet("balance")]
        public async Task<IEnumerable<BalanceDto>> GetBalanceAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var retVal = await _queryBus.QueryAsync<IEnumerable<BalanceDto>>(new UserDataByIdQuery(userId), token).ConfigureAwait(false);

            return retVal;
        }


        [HttpGet("transaction")]
        public async Task<IEnumerable<TransactionDto>> GetTransactionAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);

            var retVal = await _queryBus.QueryAsync<IEnumerable<TransactionDto>>(new UserDataByIdQuery(userId), token).ConfigureAwait(false);

            return retVal;
        }


        [HttpPost("redeem")]
        public async Task<IActionResult> RedeemAsync([FromBody]CreateRedeemRequest model,
        [FromServices] ICommandBus commandBus,
        [FromServices] IMapper mapper,
        CancellationToken token)
        {
            try
            {
                var command = new RedeemTokenCommand(_userManager.GetLongUserId(User), model.Amount);
                await commandBus.DispatchAsync(command, token).ConfigureAwait(false);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                _logger.Exception(e, new Dictionary<string, string>()
                {
                    ["model"] = model.ToString(),
                    ["user"] = _userManager.GetUserId(User)
                });
                return BadRequest();
            }
        }
    }
}