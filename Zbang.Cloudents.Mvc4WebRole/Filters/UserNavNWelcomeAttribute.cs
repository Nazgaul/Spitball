using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Helpers;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class UserNavNWelcomeAttribute : ActionFilterAttribute
    {
        private readonly IUserProfile m_UserProfile;

        public UserNavNWelcomeAttribute()
        {
            m_UserProfile = DependencyResolver.Current.GetService<IUserProfile>();
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                return;
            }
            var context = filterContext.Controller;

           // context.ViewBag.welcome = (bool)(context.TempData[Zbang.Cloudents.Mvc4WebRole.Controllers.BaseController.TempDataNameUserRegisterFirstTime] ?? false);

            var userDetail = m_UserProfile.GetUserData(context.ControllerContext);
            if (userDetail != null)
            {
                context.ViewBag.fLib = userDetail.FirstTimeLibrary;
                context.ViewBag.fDash = userDetail.FirstTimeDashboard;
                context.ViewBag.fBox = userDetail.FirstTimeBox;
                context.ViewBag.fItem = userDetail.FirstTimeItem;
            }
        }
    }
}