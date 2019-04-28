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
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

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
        private readonly ICommandBus _commandBus;

        public WalletController(UserManager<RegularUser> userManager, IQueryBus queryBus, ILogger logger, ICommandBus commandBus)
        {
            _userManager = userManager;
            _queryBus = queryBus;
            _logger = logger;
            _commandBus = commandBus;
        }

        // GET
        [HttpGet("balance")]
        public async Task<IEnumerable<BalanceDto>> GetBalanceAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var retVal = await _queryBus.QueryAsync<IEnumerable<BalanceDto>>(new UserDataByIdQuery(userId), token);

            return retVal;
        }


        [HttpGet("transaction")]
        public async Task<IEnumerable<TransactionDto>> GetTransactionAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);

            var retVal = await _queryBus.QueryAsync<IEnumerable<TransactionDto>>(new UserDataByIdQuery(userId), token);

            return retVal;
        }

        [HttpPost("BuyTokens")]
        public async Task<IActionResult> BuyTokensAsync(PayPalTransactionRequest model,
            [FromServices] IPayPal payPal, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var result = await payPal.GetPaymentAsync(model.Id);
            var command = new TransferMoneyToPointsCommand(userId, result.Amount, result.PayPalId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }


        [HttpPost("redeem")]
        public async Task<IActionResult> RedeemAsync([FromBody]CreateRedeemRequest model,
        CancellationToken token)
        {
            try
            {
                var command = new CashOutCommand(_userManager.GetLongUserId(User)/*, model.Amount*/);
                await _commandBus.DispatchAsync(command, token);
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

        #region PayMe

        /// <summary>
        /// Generate a buyer for - don't forget to run ngrok if you run it locally
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="configuration"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("GenerateSale")]
        public async Task<ActionResult<SaleResponse>> GenerateLink([FromServices] IPayment payment,
            [FromServices] IHostingEnvironment configuration,
            CancellationToken token)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user.BuyerPayment != null && user.BuyerPayment.IsValid())
            {
                return BadRequest();
            }
            var url = Url.RouteUrl("PayMeCallback", new
            {
                userId = user.Id
            }, "http");

            var uri = new UriBuilder(url);
            if (configuration.IsDevelopment())
            {
                uri.Host = "80ec9aba.ngrok.io";
                uri.Port = 80;
            };

            var result = await payment.CreateBuyerAsync(uri.Uri.AbsoluteUri, token);
            var saleUrl = new UriBuilder(result.SaleUrl);
            saleUrl.AddQuery(new NameValueCollection()
            {
                ["first_name"] = user.FirstName,
                ["last_name"] = user.LastName,
                ["phone"] = user.PhoneNumber,
                ["email"] = user.Email
            });

            return new SaleResponse(saleUrl.Uri);
        }

        [HttpPost("PayMe", Name = "PayMeCallback"), AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> PayMeCallbackAsync([FromQuery]long userId, [FromForm] PayMeCallback model, CancellationToken token)
        {
            var paymentKeyExpiration = DateTime.ParseExact(model.BuyerCardExp, "MMyy", CultureInfo.InvariantCulture);
            paymentKeyExpiration = paymentKeyExpiration.AddMonths(1).AddMinutes(-1);

            var command = new AddBuyerTokenCommand(userId, model.BuyerKey, paymentKeyExpiration);
            await _commandBus.DispatchAsync(command, token);
            //TODO: send signalR buyer exists
            return Ok();
        }
        #endregion
    }
}