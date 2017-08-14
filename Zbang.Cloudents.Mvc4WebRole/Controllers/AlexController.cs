using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class AlexController:Controller
    {
        public ActionResult Index() => View();
    }
}
