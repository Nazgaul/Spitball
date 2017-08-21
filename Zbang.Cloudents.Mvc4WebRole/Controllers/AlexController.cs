using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class AlexController:Controller
    {
        [Route("alex", Name = "Alex")]
        public ActionResult Index() => View();

        [Route("alex/faq", Name = "Alex_FAQ")]
        public ActionResult Faq()
        {
            ViewBag.page = "faq";
            return View();
        }

        [Route("alex/about", Name = "Alex_ABOUT")]
        public ActionResult About()
        {
            ViewBag.page = "about";
            return View();
        }
        [Route("alex/tos", Name = "Alex_TOS")]
        public ActionResult Terms()
        {
            ViewBag.page = "tos";
            return View();
        }
        [Route("alex/privacy", Name = "Alex_PRIVACY")]
        public ActionResult Privacy()
        {
            ViewBag.page = "privacy";
            return View();
        }
    }
}
