using System.Web.Mvc;

namespace Zbang.Zbox.Mvc4Students.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [OutputCache(Duration = 86400, VaryByParam = "none")]
        public ActionResult Index()
        {
            ViewBag.Url = Infrastructure.Extensions.ConfigFetcher.Fetch("multimiCloudUrl");
            return View();
        }
    }
}
