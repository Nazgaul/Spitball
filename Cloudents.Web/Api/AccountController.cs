using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        // GET
        public IActionResult Register()
        {
            return Json(null);
            //return View();
        }
    }
}