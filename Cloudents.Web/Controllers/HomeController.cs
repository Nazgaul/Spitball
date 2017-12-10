using Cloudents.Core;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class HomeController : Controller
    {
        [ResponseCache(Duration = TimeConst.Day)]
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}