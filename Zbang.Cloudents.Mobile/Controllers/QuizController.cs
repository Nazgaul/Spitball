using System.Web.Mvc;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.Mvc4WebRole.Controllers;
using Zbang.Cloudents.Mvc4WebRole.Filters;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class QuizController : BaseController
    {
        [RedirectToDekstopSite]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        //[UserNavNWelcome]
        [NoCache]
        public ActionResult IndexDesktop()
        {
            return new EmptyResult();
        }
       
    }
}