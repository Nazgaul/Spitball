using System.Web.Mvc;
using System.Web.SessionState;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.Mvc4WebRole.Controllers;
using Zbang.Cloudents.Mvc4WebRole.Filters;

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
        [RedirectToDekstopSite]
        public ActionResult Index()
        {
            return new EmptyResult();

        }

        
        //TODO: put output cache
        [HttpGet]
        [RedirectToDekstopSite]
        public ActionResult Choose()
        {
            return new EmptyResult();
        }
    }
}
