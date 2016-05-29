using System.Diagnostics;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Infrastructure.Consts;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ErrorController : Controller
    {
         [DonutOutputCache(Duration = TimeConst.Day, VaryByParam = "None", VaryByCustom = CustomCacheKeys.Auth + ";"
            + CustomCacheKeys.Lang)]
        public ActionResult Index()
        {
            ViewBag.postBag = true;
            Response.StatusCode = 500;
            ViewBag.errorPage = true;
            return View("error");
        }

        public ActionResult Unsupported()
        {
            ViewBag.postBag = true;
            ViewBag.errorPage = true;
            return View();

        }

        [NoCache]
        public ActionResult MembersOnly(string returnUrl)
        {
            ViewBag.postBag = true;
            ViewBag.errorPage = true;
            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View("MembersOnly");
        }

        public ActionResult NotFound()
        {
            ViewBag.postBag = true;
            Response.StatusCode = 404;
            ViewBag.errorPage = true;
            return View();
        }

    
        [HttpPost]
        public JsonResult JsLog(JavaScriptError log)
        {
            Trace.TraceWarning("js error: " + log);
            return Json(true);
        }
    }
}
