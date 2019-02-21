using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Cloudents.Web.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        internal const string Referral = "referral";
        private readonly IDataProtect _dataProtect;
        private readonly ILogger _logger;
        private readonly SignInManager<RegularUser> _signInManager;
        private readonly UserManager<RegularUser> _userManager;

        public HomeController(IDataProtect dataProtect, SignInManager<RegularUser> signInManager, UserManager<RegularUser> userManager, ILogger logger)
        {
            _dataProtect = dataProtect;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromQuery, CanBeNull] string referral,
            [FromQuery] string open,
            [FromQuery] string token
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

            if (_signInManager.IsSignedIn(User))
            {
                return View();
            }

            if (token != null)
            {
                await SignInUserAsync(token);
                return RedirectToAction("Index", new
                {
                    referral,
                    open
                });


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


        public async Task SignInUserAsync(string code)
        {
            try
            {

                var userId = _dataProtect.Unprotect(code);
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    ViewBag.Auth = true;
                    await _signInManager.SignInAsync(user, false);
                }

            }
            catch (CryptographicException ex)
            {
                //We just log the exception. user open the email too later and we can't sign it.
                //If we see this persist then maybe we need to increase the amount of time
                _logger.Exception(ex);
            }
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
            await signInManager.SignOutAsync().ConfigureAwait(false);
            TempData.Clear();


            return Redirect("/");
        }
    }
}