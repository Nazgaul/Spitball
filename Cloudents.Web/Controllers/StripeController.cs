using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Cloudents.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StripeController : Controller
    {
        private readonly IStripeService _stripeService;
        private readonly ICommandBus _commandBus;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public StripeController(IStripeService stripeService, ICommandBus commandBus, UserManager<User> userManager, IConfiguration configuration)
        {
            _stripeService = stripeService;
            _commandBus = commandBus;
            _userManager = userManager;
            _configuration = configuration;
        }

        [Route("BuyPoints", Name = "stripe-buy-points")]
        public async Task<IActionResult> StripeCallbackBuyPointsAsync(
            string redirectUrl, string sessionId,
            CancellationToken token)
        {
            var (receipt, points) = await _stripeService.GetBuyPointDataByIdAsync(sessionId, token);

            var userId = _userManager.GetLongUserId(User);
            var command = new TransferMoneyToPointsCommand(userId, points, receipt);
            await _commandBus.DispatchAsync(command, token);
            return Redirect(redirectUrl);
        }



        [Route("Subscribe", Name = "stripe-subscribe")]
        public async Task<IActionResult> StripeCallSubscribeAsync(
            string redirectUrl, string sessionId,
            CancellationToken token)
        {
            var tutorId = await _stripeService.GetSubscriptionByIdAsync(sessionId, token);

            var userId = _userManager.GetLongUserId(User);

            var command = new SubscribeToTutorCommand(userId, tutorId);
            //var command = new TransferMoneyToPointsCommand(userId, points, receipt);
            await _commandBus.DispatchAsync(command, token);
            return Redirect(redirectUrl);
        }

        [Route("stripe-finish-connect", Name = "stripe-finish-connect")]
        public async Task<IActionResult> StripeFinishConnect([FromQuery]string code, CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User);
            var stripeUserId = await _stripeService.GetStripeUserIdAsync(code, token);
            var command = new AddSellerTokenCommand(user.Email, stripeUserId);
            await _commandBus.DispatchAsync(command, token);
            return RedirectToAction("Index", "Home");
        }


        [Route("stripe-connect")]
        public async Task<RedirectResult> StripeConnect()
        {
            var clientId = _configuration["Stripe:Connect"];
            var user = await _userManager.GetUserAsync(User);
            if (user.Tutor == null)
            {
                return Redirect("/");
            }

            if (user.SbCountry != Country.UnitedStates)
            {
                return Redirect("/");

            }
            var homePage = Url.RouteUrl("stripe-finish-connect");
            var profileUrl = Url.RouteUrl(SeoTypeString.Tutor, new
            {
                id = user.Id,
                name = FriendlyUrlHelper.GetFriendlyTitle(user.Name)
            }, "https");
            var queryParams = new NameValueCollection()
            {
                ["redirect_uri"] = homePage,
                ["client_id"] = clientId,
                ["stripe_user[email]"] = user.Email,
                ["stripe_user[url]"] = profileUrl,
                ["stripe_user[phone_number]"] = user.PhoneNumber,
                ["stripe_user[first_name]"] = user.FirstName,
                ["stripe_user[last_name]"] = user.LastName

            };
            var uri = new UriBuilder("https://connect.stripe.com/express/oauth/authorize");
            uri.AddQuery(queryParams);
            return Redirect(uri.ToString());
        }
    }
}
