using System;
using System.Web.Mvc;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.Mvc3WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            
            return View("Error");
        }
    }
}
