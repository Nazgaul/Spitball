using System.Diagnostics;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ErrorController : BaseController
    {

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
                return RedirectToAction("Index", "Dashboard");
            }
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
