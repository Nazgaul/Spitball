using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    public class StartController : Controller
    {
        [AllowAnonymous]
        [DonutOutputCache(CacheProfile = "FullPage")]
        [Route("Start", Name = "start")]
        // GET: Start
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("homePage");
            }
            return View("Empty");
        }

        [HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult IndexPartial()
        {
            return PartialView("Index");
        }
    }
}