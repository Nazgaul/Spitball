using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Web.Filters;
using Cloudents.Web.Hubs;
using Cloudents.Web.Models;
using JetBrains.Annotations;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        internal const string Referral = "referral";
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public HomeController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        //Any got issue with auth vs no auth. need to fix this.
        // [ResponseCache(Location = ResponseCacheLocation.Client, Duration = TimeConst.Hour, VaryByQueryKeys = new[] { "*" })]
        //this is due to issue with log in state
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [SignInWithToken]
        [ApiNotFoundFilter]
        public IActionResult Index(
            [FromQuery, CanBeNull] string referral,
            [FromQuery] string open
            )
        {
            if (!string.IsNullOrEmpty(referral))
            {
                TempData[Referral] = referral;
            }

            if (_signInManager.IsSignedIn(User))
            {
                return View();
            }

            if (open?.Equals("referral", StringComparison.OrdinalIgnoreCase) == true)
            {
                return RedirectToAction("Index", new
                {
                    referral
                });
            }


            return View();
        }

        [Route("logout")]
        public async Task<IActionResult> LogOutAsync(
            [FromServices] SignInManager<User> signInManager,
            [FromServices] IHubContext<SbHub> hubContext, CancellationToken token)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            var message = new SignalRTransportType(SignalRType.User, SignalREventAction.Logout,
                new object());

            await hubContext.Clients.User(signInManager.UserManager.GetUserId(User)).SendCoreAsync("Message", new object[]
            {
                message
            }, token);
            await signInManager.SignOutAsync();
            TempData.Clear();


            return Redirect("/");
        }



        [Route("image/{hash}", Name = "imageUrl")]
        [ResponseCache(
            Duration = TimeConst.Month, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "*" })]
        public IActionResult ImageRedirect([FromRoute]string hash, [FromServices] IConfiguration configuration)

        {
            string val = Request.QueryString.ToUriComponent();
            if (!val.StartsWith("?"))
            {
                val = $"?{val}";
            }
            return Redirect(
                $"{configuration["functionCdnEndpoint"]}/api/image/{hash}{val}");
        }

        internal const string PaymeCallbackRouteName = "ReturnUrl";
        internal const string PaymeCallbackRouteName2 = "ReturnUrl2";
        [Route("PaymentProcessing", Name = PaymeCallbackRouteName)]
        public async Task<IActionResult> Processing(PaymeSuccessCallback model,
            [FromServices] ICommandBus commandBus,
            [FromServices] TelemetryClient logger,
            CancellationToken token)
        {
            if (model.Status.Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                var command = new ConfirmPaymentCommand(model.UserId);
                await commandBus.DispatchAsync(command, token);
            }
            else
            {
                var values = Request.Form.ToDictionary(s => s.Key, x => x.Value.ToString());
                values.Add("userId", model.UserId.ToString());
                logger.TrackTrace("Credit Card Process Failed", values);
            }
            return View("Processing", model);
        }


        [Route("PaymentProcessing2", Name = PaymeCallbackRouteName2)]
        public async Task<IActionResult> BuyTokensProcessing(PaymeSuccessCallback model, [FromQuery]int points,
            [FromServices] ICommandBus commandBus,
            [FromServices] TelemetryClient logger,
            CancellationToken token)
        {
            if (model.Status.Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                var command = new TransferMoneyToPointsCommand(model.UserId, points, model.TransactionId);
                await commandBus.DispatchAsync(command, token);
            }
            else
            {
                var values = Request.Form.ToDictionary(s => s.Key, x => x.Value.ToString());
                values.Add("userId", model.UserId.ToString());
                logger.TrackTrace("Credit Card Process Failed", values);
            }
            return View("Processing", model);
        }


        [Route("google")]
        public async Task<RedirectToRouteResult> GoogleSigninAndroidAsync(string token,
            [FromServices] IGoogleAuth service,
            [FromServices] IDataProtectionProvider dataProtectProvider,
            CancellationToken cancellationToken
            )
        {
            var result = await service.LogInAsync(token, cancellationToken);
            if (result == null)
            {
                return RedirectToRoute("Index");
            }

            var result2 = await _signInManager.ExternalLoginSignInAsync("Google", result.Id, true, true);

            var user2 = await _userManager.FindByEmailAsync(result.Email);
            var dataProtector = dataProtectProvider.CreateProtector("Spitball").ToTimeLimitedDataProtector();
            var code = dataProtector.Protect(user2.ToString(), DateTimeOffset.UtcNow.AddDays(5));
            return RedirectToRoute("Index", new
            {
                token = code
            });
        }
    }
}