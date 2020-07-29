using Cloudents.Command;
using Cloudents.Command.Command;
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
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Enum;
using Cloudents.Core.Query.Payment;
using Cloudents.Query.Courses;
using Cloudents.Query.Tutor;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]
    public class WalletController : ControllerBase
    {
        public const string StudyroomIdMetaData = "StudyRoomId";
        private readonly IQueryBus _queryBus;
        private readonly UserManager<User> _userManager;
        private readonly ILogger _logger;
        private readonly ICommandBus _commandBus;
        private readonly Lazy<IPaymeProvider> _payment;

        public WalletController(UserManager<User> userManager, IQueryBus queryBus, ILogger logger, ICommandBus commandBus, Lazy<IPaymeProvider> payment)
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

        //[HttpPost("BuyTokens")]
        //public async Task<SaleResponse> BuyTokensAsync(BuyPointsRequest model, CancellationToken token)
        //{

        //    var user = await _userManager.GetUserAsync(User);
        //    var urlReturn = Url.RouteUrl(HomeController.PaymeCallbackRouteName2, new
        //    {
        //        userId = user.Id,
        //        points = model.Points
        //    }, "https");

        //    var bundle = Enumeration.FromValue<PointBundle>(model.Points);
        //    var result = await _payment.Value.BuyTokens(bundle!, urlReturn, token);
        //    var saleUrl = new UriBuilder(result.SaleUrl);
        //    saleUrl.AddQuery(new NameValueCollection()
        //    {
        //        ["first_name"] = user.FirstName,
        //        ["last_name"] = user.LastName,
        //        ["phone"] = user.PhoneNumber,
        //        ["email"] = user.Email,
        //    });
        //    return new SaleResponse(saleUrl.Uri);
        //}

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
                if (user.PaymentExists == PaymentStatus.Done)
                {
                    return BadRequest("Already have payment");
                }

                var url = Url.RouteUrl("PayMeCallback", new
                {
                    userId = user.Id
                }, "https");


                var urlReturn = Url.RouteUrl(PaymeController.PaymeCallbackRouteName, new
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
        public async Task<IActionResult> PayMeCallbackAsync([FromQuery] long userId,
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




        [HttpPost("Payme/Course/{id:long}")]
        public async Task<IActionResult> PaymeAsync(long id,
            [FromServices] IPaymeProvider paymeProvider,
            [FromHeader(Name = "referer")] string referer,
            CancellationToken token)
        {

            var user = await _userManager.GetUserAsync(User);
            var query = new CourseByIdQuery(id, user.Id);
            var courseDetail = await _queryBus.QueryAsync(query, token);

            var urlReturn = Url.RouteUrl(PaymeController.EnrollStudyRoom, new
            {
                userId = user.Id,
                courseId = id,
                redirectUrl = referer
            }, "https");
            var result = await paymeProvider.BuyCourseAsync(courseDetail.Price, courseDetail.Name, urlReturn,
                courseDetail.TutorSellerKey, token);
            
            return Ok(new
            {
                sessionId = result.SaleUrl
            });


        }


        #endregion



        #region Stripe
        [HttpPost("Stripe/Course/{id:long}")]
        public async Task<IActionResult> StripeAsync(long id,
            [FromHeader(Name = "referer")] string referer,
            [FromServices] IStripeService service,
            CancellationToken token)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var userId = _userManager.GetLongUserId(User);
            var query = new CourseByIdQuery(id, userId);
            var studyRoomResult = await _queryBus.QueryAsync(query, token);

            var uriBuilder = new UriBuilder(referer)
            {
                Query = string.Empty
            };

            var url = new UriBuilder(Url.RouteUrl(StripeController.EnrollStudyRoom, new
            {
                redirectUrl = uriBuilder.ToString()
            }, "https"));

            var successCallback = url.AddQuery(("sessionId", "{CHECKOUT_SESSION_ID}"), false).ToString();

            var stripePaymentRequest = new StripePaymentRequest(studyRoomResult.Name,
                studyRoomResult.Price,
                email,
                successCallback,
                referer)
            {
                Metadata = new Dictionary<string, string>()
                {
                    [StudyroomIdMetaData] = id.ToString(),
                    ["UserId"] = userId.ToString()
                }
            };

            var result = await service.CreatePaymentAsync(stripePaymentRequest, token);
            return Ok(new
            {
                sessionId = result
            });
        }

        #endregion

    }
}