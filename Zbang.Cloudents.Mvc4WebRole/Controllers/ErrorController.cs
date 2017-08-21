using System.Diagnostics;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ErrorController : Controller
    {
        [DonutOutputCache(CacheProfile = "FullPage")]
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
            Trace.TraceWarning($"userId: {User.GetUserId(false)} js error: {log}");
            return Json(true);
        }
    }
}
