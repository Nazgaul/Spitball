using System.Web.Mvc;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.SiteExtension;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class QuizController : BaseController
    {
        [RedirectToDesktopSite]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        //[UserNavNWelcome]
        [NoCache]
        public ActionResult IndexDesktop()
        {
            return new EmptyResult();
        }
       
    }
}