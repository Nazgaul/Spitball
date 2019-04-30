using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ForgotPasswordController : Controller
    {
        // GET
        [Route("resetPassword", Name = "ResetPassword")]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoute(HomeController.RootRoute);
            }
            return View();
        }
    }
}