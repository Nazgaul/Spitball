﻿using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index(ConfirmEmailRequest model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index), "Home");
            }
            model.Code = System.Net.WebUtility.UrlDecode(model.Code);
            var user = await _userManager.FindByIdAsync(model.Id.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{model.Id}'.");
            }

            if (user.PhoneNumberConfirmed)
            {
                return RedirectToRoute(RegisterController.Signin);
            }
            if (user.EmailConfirmed)
            {
                return await GoToStep(user, NextStep.EnterPhone, false, model.ReturnUrl);
            }
            var result = await _userManager.ConfirmEmailAsync(user, model.Code).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                _logger.Error($"Error confirming email for user with ID '{model.Id}': {result}, User: {user}");
                return RedirectToRoute(RegisterController.RegisterRouteName, new { step = "expiredStep" });
            }
          
            TempData[HomeController.Referral] = model.Referral;

            return await GoToStep(user, NextStep.EnterPhone, true, model.ReturnUrl);
        }

        private async Task<RedirectToRouteResult> GoToStep(User user, NextStep step, bool isNew, string returnUrl)
        {
            await _signInManager.SignInTwoFactorAsync(user, false).ConfigureAwait(false);
            return RedirectToRoute(RegisterController.RegisterRouteName,
                new
                {
                    step,
                    isNew,
                    returnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : null
                });
        }
    }
}