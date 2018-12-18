using Cloudents.Core;
using Cloudents.Domain.Entities;
using Cloudents.Web.Hubs;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        internal const string Referral = "referral";

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        //[ServiceFilter(typeof(RedirectToOldSiteFilterAttribute))]
        public IActionResult Index(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromQuery, CanBeNull] string referral
            )
        {
            if (!string.IsNullOrEmpty(referral))
            {
                TempData[Referral] = referral;
            }

            if (userAgent != null && userAgent.Contains("linkedin", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.fbImage = ViewBag.imageSrc = "/images/3rdParty/linkedinShare.png";
            }
            return View();
        }

        [Route("logout")]
        public async Task<IActionResult> LogOutAsync(
            [FromServices] SignInManager<RegularUser> signInManager,
            [FromServices] IHubContext<SbHub> hubContext, CancellationToken token)
        {
            await signInManager.SignOutAsync().ConfigureAwait(false);
            TempData.Clear();

            var message = new SignalRTransportType(SignalRType.User, SignalREventAction.Logout,
                new object());
            await hubContext.Clients.User(signInManager.UserManager.GetUserId(User)).SendCoreAsync("Message", new object[]
                        {
                            message
                        }, token);
            return Redirect("/");
        }
    }
}