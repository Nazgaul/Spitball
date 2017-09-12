using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web2.Controllers
{
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return
            View();
        }

        public IActionResult GetMeAjaxBicth()
        {
            return Content("I dont want care");
        }
    }
}