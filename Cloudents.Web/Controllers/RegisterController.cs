using Cloudents.Core.Entities;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

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
        [Route("register", Name = RegisterRouteName)]
        [Route("signin", Name = Signin)]
        public async Task<IActionResult> Index(NextStep? step, CancellationToken token)
        {


            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }

            if (!step.HasValue) return View();
            switch (step.Value)
            {
                case NextStep.EmailConfirmed:
                    var val = TempData.Peek(Api.RegisterController.Email);
                    if (val == null)
                    {
                        return RedirectToRoute(RegisterRouteName);
                    }
                    break;
                case NextStep.VerifyPhone:
                    var userVerified = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                    if (userVerified == null)
                    {
                        return RedirectToRoute(RegisterRouteName);
                    }



                    break;
                case NextStep.EnterPhone:
                    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                    if (user == null)
                    {
                        return RedirectToRoute(RegisterRouteName);
                    }

                    if (user.PhoneNumber != null && !user.PhoneNumberConfirmed)
                    {
                        await _client.SendSmsAsync(user, token);
                        return RedirectToRoute(RegisterRouteName, new { step = NextStep.VerifyPhone });
                    }

                    break;
            }
            return View();
        }
    }
}