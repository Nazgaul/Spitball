using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Admin2.Controllers
{
    public class HomeController : Controller
    {

        //[Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
