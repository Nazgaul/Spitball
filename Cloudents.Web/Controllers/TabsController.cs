using System;
using Cloudents.Web.Binders;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Controllers
{
    public class TabsController : Controller
    {
        // GET: /<controller>/
        [Route("tutor")]
        [Route("book")]
        [Route("job")]
        public IActionResult Index(
            [ModelBinder(typeof(CountryModelBinder))] string country
            )
        {
            if (string.Equals(country, "il", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}
