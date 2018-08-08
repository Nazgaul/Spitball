using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RegisterController : Controller
    {
        // GET
        [Route("register")]
        [Route("signin")]
        [HttpGet]
        public IActionResult Redirect()
        {
            return RedirectToRoute("login");
        }


        [Route("login",Name="login")]
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