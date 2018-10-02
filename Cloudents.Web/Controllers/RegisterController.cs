using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RegisterController : Controller
    {

        internal const string RegisterRouteName = "Register";

        internal const string Signin = "SignIn";

        // GET
        [Route("register", Name = RegisterRouteName)]
        [Route("signin", Name = Signin)]
        [Route("resetPassword", Name = "ResetPassword")]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}