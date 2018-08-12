using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Api;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            //if (model.Id == null || model.Code == null)
            //{
            //    return RedirectToAction(nameof(Index), "Home");
            //}
            TempData.Remove(SignUserController.Email);
            model.Code = System.Net.WebUtility.UrlDecode(model.Code);
            var user = await _userManager.FindByIdAsync(model.Id.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{model.Id}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, model.Code).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                _logger.Error($"Error confirming email for user with ID '{model.Id}': {result}");
                return RedirectToRoute("Register", new { step = "expiredStep" });
            }

            await _signInManager.SignInTwoFactorAsync(user, false).ConfigureAwait(false);

            return RedirectToRoute("Register", 
                new {
                    step = "enterPhone",
                    newUser = true,
                    returnUrl = Url.IsLocalUrl(model.ReturnUrl) ? model.ReturnUrl : null
            });
            //return Redirect("/register?newUser&step=enterPhone");
        }
    }
}