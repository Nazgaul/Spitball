using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    
    public class QuizController : Controller
    {
        //
        // GET: /Quiz/
        public ActionResult Index()
        {
            return View();
        }
	}
}