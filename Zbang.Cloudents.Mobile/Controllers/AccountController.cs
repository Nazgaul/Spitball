using System.Threading;
using System.Web.Mvc;
using System.Web.UI;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mobile.Models.Account;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Url;

namespace Zbang.Cloudents.Mobile.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [DonutOutputCache(VaryByParam = "lang", VaryByCustom = CustomCacheKeys.Auth + ";"
           + CustomCacheKeys.Lang + ";"
           + CustomCacheKeys.Mobile,
           Duration = TimeConsts.Minute * 5,
           Location = OutputCacheLocation.Server
           )]
        [PreserveQueryString]
        public ActionResult Index(string lang, string invId)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            if (!string.IsNullOrEmpty(invId))
            {
                var guid = GuidEncoder.TryParseNullableGuid(invId);
                if (guid.HasValue)
                {
                    var h = new CookieHelper(HttpContext);
                    h.InjectCookie(Invite.CookieName, new Invite { InviteId = guid.Value });
                }
            }
            if (lang != null && lang != Thread.CurrentThread.CurrentUICulture.Name)
            {
                RouteData.Values.Remove("lang");
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Welcome(string universityId)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View(new LogOnRegister { LogOn = new LogOn(), Register = new Register() });
        }
    }
}