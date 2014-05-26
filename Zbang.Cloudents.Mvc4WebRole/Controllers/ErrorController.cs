using System.Web.Mvc;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ErrorController : BaseController
    {
        public ErrorController()
        {

        }
        public ErrorController(IZboxWriteService zboxWriteService, IZboxReadService zboxReadService,
                              // IShortCodesCache shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService)
            : base(zboxWriteService, zboxReadService, 
            //shortToLongCache,
            formsAuthenticationService)
        {
        }

        public ActionResult Index()
        {
            return View("error");
        }

        public ActionResult Unsupported()
        {
            return View();

        }

        //public ActionResult NotFound()
        //{
        //    return View();
        //}

        public ActionResult MembersOnly()
        {
            return View("MembersOnly");
        }
    }
}
