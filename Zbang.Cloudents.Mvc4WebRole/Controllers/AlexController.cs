using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [RoutePrefix("new")]
    public class AlexController:Controller
    {
        [Route(Name = "Alex")]
        public ActionResult Index() => View();

        [Route("faq", Name = "Alex_FAQ")]
        public ActionResult Faq()
        {
            ViewBag.page = "faq";
            return View();
        }

        [Route("about", Name = "Alex_ABOUT")]
        public ActionResult About()
        {
            ViewBag.page = "about";
            return View();
        }

        [Route("tos", Name = "Alex_TOS")]
        public ActionResult Terms()
        {
            ViewBag.page = "tos";
            return View();
        }

        [Route("privacy", Name = "Alex_PRIVACY")]
        public ActionResult Privacy()
        {
            ViewBag.page = "privacy";
            return View();
        }
        [Route("competiton", Name = "Alex_MONEY")]
        public ActionResult WinMoney()
        {
            ViewBag.page = "money";
            return View();
        }
    }
}
