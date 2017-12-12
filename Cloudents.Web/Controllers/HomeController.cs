using Cloudents.Core;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class HomeController : Controller
    {
        //[ResponseCache()] 
      // we can't use that for now.
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}