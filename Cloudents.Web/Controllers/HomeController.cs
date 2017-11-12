using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}