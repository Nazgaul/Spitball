﻿using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class AlexController:Controller
    {
        [Route("alex", Name = "Alex")]
        public ActionResult Index() => View();
        [Route("alex/faq", Name = "Alex_FAQ")]
        public ActionResult Faq() => View();
        [Route("alex/about", Name = "Alex_ABOUT")]
        public ActionResult About() => View();

    }
}
