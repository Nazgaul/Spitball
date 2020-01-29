using Cloudents.Core.Entities;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using WebMarkupMin.Core;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RegisterController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ISmsSender _client;

        public RegisterController(SignInManager<User> signInManager, ISmsSender client)
        {
            _signInManager = signInManager;
            _client = client;
        }

        internal const string RegisterRouteName = "Register";

        internal const string Signin = "SignIn";

        // GET
        [Route("register/{page?}", Name = RegisterRouteName)]
        [Route("signin/{page?}", Name = Signin)]
        public async Task<IActionResult> IndexAsync(string page, /*NextStep? step,*/ CancellationToken token)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/feed");
            }



            var step = RegistrationStep.GetStepByUrl(page);
            if (step is null)
            {
                return View("Index");

            }

            if (step.Equals(RegistrationStep.RegisterEmailConfirmed))
            {
                var val = TempData.Peek(Api.RegisterController.Email);
                if (val is null)
                {
                    return RedirectToRoute(RegisterRouteName);
                }
            }

            if (step.Equals(RegistrationStep.RegisterVerifyPhone))
            {
                var userVerified = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (userVerified is null)
                {
                    return RedirectToRoute(RegisterRouteName);
                }
            }

            if (step.Equals(RegistrationStep.RegisterSetPhone))
            {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user is null)
                {
                    return RedirectToRoute(RegisterRouteName);
                }

                if (user.PhoneNumber != null && !user.PhoneNumberConfirmed)
                {
                    await _client.SendSmsAsync(user, token);
                    return RedirectToRoute(RegisterRouteName, new
                    {
                        page = RegistrationStep.RegisterVerifyPhone.RouteName
                    });
                }
            }

            return View("Index");
        }
    }
}