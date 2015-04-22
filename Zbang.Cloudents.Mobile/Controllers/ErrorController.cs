using System.Diagnostics;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.Mobile.Helpers;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Cloudents.Mobile.Models;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ErrorController : BaseController
    {
         [DonutOutputCache(Duration = TimeConsts.Day, VaryByParam = "None", VaryByCustom = CustomCacheKeys.Auth + ";"
            + CustomCacheKeys.Lang)]
        public ActionResult Index()
        {
            return View("error");
        }

        public ActionResult Unsupported()
        {
            return View();

        }

        [NoCache]
        public ActionResult MembersOnly(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToRoute("dashboardLink");
            }
            //return View("error");
            return View("MembersOnly");
        }

        [HttpPost]
        public JsonResult JsLog(JavaScriptError log)
        {
            Trace.TraceWarning("js error: " + log);
            return Json(true);
        }
    }
}
