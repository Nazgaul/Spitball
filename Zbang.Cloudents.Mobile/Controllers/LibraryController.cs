﻿using System.Web.Mvc;
using System.Web.SessionState;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.Mvc4WebRole.Controllers;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    public class LibraryController : BaseController
    {

        public ActionResult DepartmentRedirect()
        {
            return RedirectToRoutePermanent("Default", new { controller = "Library", Action = "Index" });
        }

        //[UserNavNWelcome]
        [HttpGet]
        [RedirectToDesktopSite]
        public ActionResult Index()
        {
            return new EmptyResult();

        }


        public ActionResult ChoosePartial()
        {
            return PartialView("ss");
        }
    }
}
