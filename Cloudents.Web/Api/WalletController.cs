using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletController : Controller
    {
        private readonly IQueryBus _queryBus;
        private readonly UserManager<User> _userManager;
        private readonly ILogger _logger;

        public WalletController(UserManager<User> userManager, IQueryBus queryBus, ILogger logger)
        {
            _userManager = userManager;
            _queryBus = queryBus;
            _logger = logger;
        }

        // GET
        [HttpGet("balance")]
        public async Task<IActionResult> GetBalanceAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var retVal = await _queryBus.QueryAsync<IEnumerable<BalanceDto>>(new UserDataByIdQuery(userId), token).ConfigureAwait(false);

            return Ok(retVal);
        }


        [HttpGet("transaction")]
        public async Task<IActionResult> GetTransactionAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);

            var retVal = await _queryBus.QueryAsync<IEnumerable<TransactionDto>>(new UserDataByIdQuery(userId), token).ConfigureAwait(false);

            return Ok(retVal);
        }


        [HttpPost("redeem"), ValidateModel]
        public async Task<IActionResult> RedeemAsync([FromBody]CreateRedeemRequest model,
        [FromServices] ICommandBus commandBus,
        [FromServices] IMapper mapper,
        CancellationToken token)
        {
            try
            {
                var command = mapper.Map<RedeemTokenCommand>(model);
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