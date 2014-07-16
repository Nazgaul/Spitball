using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Security;
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

        [CacheFilter]
        public ActionResult MembersOnly(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View("MembersOnly");
        }
    }
}
