using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Cloudents.Web.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Web.Filters;
using Wangkanai.Detection;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        internal const string Referral = "referral";
        private readonly SignInManager<RegularUser> _signInManager;

        public HomeController(SignInManager<RegularUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true), SignInWithToken]
        public IActionResult Index(
            [FromServices] ICrawlerResolver crawlerResolver,
            //  [FromHeader(Name = "User-Agent")] string userAgent,
            [FromQuery, CanBeNull] string referral,
            [FromQuery] string open
            )
        {
            if (!string.IsNullOrEmpty(referral))
            {
                TempData[Referral] = referral;
            }
            if (crawlerResolver?.Crawler?.Type == CrawlerType.LinkedIn)
            {
                ViewBag.fbImage = ViewBag.imageSrc = "/images/3rdParty/linkedinShare.png";
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
            [FromServices] SignInManager<RegularUser> signInManager,
            [FromServices] IHubContext<SbHub> hubContext, CancellationToken token)
        {

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
            Duration = TimeConst.Month, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new []{"*"})]
        public IActionResult ImageRedirect([FromRoute]string hash, [FromServices] IConfiguration configuration)
        {
            return Redirect(
                $"{configuration["functionCdnEndpoint"]}/api/image/{hash}?{Request.QueryString}");
        }


    }
}