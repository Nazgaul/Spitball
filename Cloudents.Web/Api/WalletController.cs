using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.Users;
using Cloudents.Web.Controllers;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
        private readonly UserManager<User> _userManager;
        private readonly ILogger _logger;
        private readonly ICommandBus _commandBus;
        private readonly Lazy<IPayment> _payment;

        public WalletController(UserManager<User> userManager, IQueryBus queryBus, ILogger logger, ICommandBus commandBus, Lazy<IPayment> payment)
        {
            _userManager = userManager;
            _queryBus = queryBus;
            _logger = logger;
            _commandBus = commandBus;
            _payment = payment;
        }

        // GET
        [HttpGet("balance")]
        public async Task<IEnumerable<BalanceDto>> GetBalanceAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var retVal = await _queryBus.QueryAsync(new UserBalanceQuery(userId), token);

            return retVal;
        }


        [HttpGet("transaction")]
        public async Task<IEnumerable<TransactionDto>> GetTransactionAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);

            var retVal = await _queryBus.QueryAsync(new UserTransactionQuery(userId), token);

            return retVal;
        }




        [HttpPost("redeem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RedeemAsync(CancellationToken token)
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
                    ["user"] = _userManager.GetUserId(User)
                });
                return BadRequest();
            }
        }

        #region PayMe

        [HttpPost("BuyTokens")]
        public async Task<SaleResponse> BuyTokensAsync(BuyPointsRequest model, CancellationToken token)
        {

            var user = await _userManager.GetUserAsync(User);
            var urlReturn = Url.RouteUrl(HomeController.PaymeCallbackRouteName2, new
            {
                userId = user.Id,
                points = model.Points
            }, "https");

            var result = await _payment.Value.BuyTokens(PointBundle.Parse(model.Points), urlReturn, token);
            var saleUrl = new UriBuilder(result.SaleUrl);
            saleUrl.AddQuery(new NameValueCollection()
            {
                ["first_name"] = user.FirstName,
                ["last_name"] = user.LastName,
                ["phone"] = user.PhoneNumber,
                ["email"] = user.Email,
            });
            return new SaleResponse(saleUrl.Uri);
        }

        /// <summary>
        /// Generate a buyer for - don't forget to run ngrok if you run it locally
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("GetPaymentLink")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<SaleResponse>> GenerateLink(
            CancellationToken token)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user.BuyerPayment != null && user.BuyerPayment.IsValid())
                {
                    throw new ArgumentException();
                }

                var url = Url.RouteUrl("PayMeCallback", new
                {
                    userId = user.Id
                }, "https");


                var urlReturn = Url.RouteUrl(HomeController.PaymeCallbackRouteName, new
                {
                    userId = user.Id
                }, "https");

                var result = await _payment.Value.CreateBuyerAsync(url, urlReturn, token);
                var saleUrl = new UriBuilder(result.SaleUrl);
                saleUrl.AddQuery(new NameValueCollection()
                {
                    ["first_name"] = user.FirstName,
                    ["last_name"] = user.LastName,
                    ["phone"] = user.PhoneNumber,
                    ["email"] = user.Email,
                });
                return new SaleResponse(saleUrl.Uri);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        [HttpPost("PayMe", Name = "PayMeCallback"), AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> PayMeCallbackAsync([FromQuery]long userId,
            [FromForm] PayMeBuyerCallbackRequest model,
            [FromServices] TelemetryClient client,
            CancellationToken token)
        {
            var paymentKeyExpiration = DateTime.ParseExact(model.BuyerCardExp, "MMyy", CultureInfo.InvariantCulture);
            paymentKeyExpiration = paymentKeyExpiration.AddMonths(1).AddMinutes(-1);
            client.TrackTrace("Receive credit card details", new Dictionary<string, string>()
            {
                ["userId"] = userId.ToString(),
                ["data"] = model.ToString()
            });
            var command = new AddBuyerTokenCommand(userId, model.BuyerKey, paymentKeyExpiration, model.BuyerCardMask);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }


        [HttpPost("Seller"), AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PayMeSellerBackAsync([FromForm] PayMeSellerCallbackRequest model,
            [FromServices] TelemetryClient logger,
            CancellationToken token)
        {

            try
            {
                var command = new AddSellerTokenCommand(model.Email, model.SellerKey);
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (NullReferenceException e)
            {
                logger.TrackException(e, model.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).ToDictionary
                (
                    propInfo => propInfo.Name,
                    propInfo => propInfo.GetValue(model, null)?.ToString()

                ));
                return Ok();
            }
        }




        #endregion


        #region PayPal

        [HttpPost("PayPal/StudyRoom")]
        public async Task<IActionResult> PayPal(PayPalOrderRequest model,
            //[FromServices] IPayPal payPalService,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new AddPayPalOrderCommand(userId, model.OrderId, model.SessionId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }


        [HttpPost("PayPal/BuyTokens")]
        public async Task<IActionResult> BuyTokensAsync(PayPalTransactionRequest model,
            [FromServices] IPayPalService payPal, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var result = await payPal.GetPaymentAsync(model.Id, token);


            var amount = result.ReferenceId switch
            {
                "points_1" => 100,
                "points_2" => 500,
                "points_3" => 1000,
                _ => throw new ArgumentException(message: "invalid value")
            };


            var command = new TransferMoneyToPointsCommand(userId, amount, model.Id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
        #endregion
    }
}