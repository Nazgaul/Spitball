using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;

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

        [Ajax, HttpPost]
        public ActionResult JsLog(JavaScriptError log)
        {
            TraceLog.WriteError("js error: " + log);
            return this.CdJson(true);
        }
    }
}
