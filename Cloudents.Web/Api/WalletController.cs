﻿using AutoMapper;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        [HttpPost("transaction")]
        public async Task<IActionResult> ValidatePaymentAsync(PayPalTransactionRequest model,
            [FromServices] IPayPal payPal, CancellationToken token)
        {
            var result = await payPal.GetPaymentAsync(model.Id);
            return Ok();
        }


        [HttpPost("redeem")]
        public async Task<IActionResult> RedeemAsync([FromBody]CreateRedeemRequest model,
        [FromServices] ICommandBus commandBus,
        [FromServices] IMapper mapper,
        CancellationToken token)
        {
            try
            {
                var command = new CashOutCommand(_userManager.GetLongUserId(User), model.Amount);
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