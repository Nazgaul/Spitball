using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using SbSignInManager = Cloudents.Web.Identity.SbSignInManager;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]", Name = "ConfirmEmail")]
    public class ConfirmEmailController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SbSignInManager _signInManager;
        private readonly ILogger _logger;


        public ConfirmEmailController(UserManager<User> userManager, SbSignInManager signInManager, ILogger logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // GET
        public async Task<IActionResult> IndexAsync(ConfirmEmailRequest model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return Redirect("/");
            }

            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            model.Code = System.Net.WebUtility.UrlDecode(model.Code);
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user is null)
            {
                throw new ApplicationException($"Unable to load user with ID '{model.Id}'.");
            }

            if (user.PhoneNumberConfirmed)
            {
                return RedirectToRoute(RegisterController.Signin);
            }
            if (user.EmailConfirmed)
            {
                return await GoToStepAsync(user, RegistrationStep.RegisterSetPhone, model.ReturnUrl);
            }
            var result = await _userManager.ConfirmEmailAsync(user, model.Code);
            if (!result.Succeeded)
            {
                _logger.Error($"Error confirming email for user with ID '{model.Id}': {result}, User: {user}");
                return RedirectToRoute(RegisterController.RegisterRouteName,
                    new
                    {
                        //There was an extra step in here
                        page = RegistrationStep.RegisterSetEmailPassword
                    });
            }

            TempData[HomeController.Referral] = model.Referral;

            return await GoToStepAsync(user,
                RegistrationStep.RegisterSetPhone,
                model.ReturnUrl);
        }

        private async Task<RedirectToRouteResult> GoToStepAsync(User user, RegistrationStep step, string returnUrl)
        {
            await _signInManager.TempSignIn(user);
            var v =  RedirectToRoute(RegisterController.RegisterRouteName,
                new
                {
                    page = step.RoutePath,
                    returnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : null
                });
            return v;
        }
    }
}