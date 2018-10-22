using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Web.Binders;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RegisterController : Controller
    {
        private readonly SignInManager<User> _signInManager;

        public RegisterController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        internal const string RegisterRouteName = "Register";

        internal const string Signin = "SignIn";

        // GET
        [Route("register", Name = RegisterRouteName)]
        [Route("signin", Name = Signin)]
        public async Task<IActionResult> Index(NextStep? step,
            [ModelBinder(typeof(CountryModelBinder))] string country)
        {
            ViewBag.country = country ?? "us";
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
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
                case NextStep.EnterPhone:
                    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                    if (user == null)
                    {
                        return RedirectToRoute(RegisterRouteName);
                    }

                    break;
            }
            return View();
        }
    }
}