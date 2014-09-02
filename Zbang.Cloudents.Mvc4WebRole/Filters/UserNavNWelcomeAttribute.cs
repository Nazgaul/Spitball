using System.Web.Mvc;
using Newtonsoft.Json;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
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
            if (userDetail == null) return;

            var serializer = new JsonNetSerializer();
            var retVal = serializer.Serialize(new
              {
                  dashboard = userDetail.FirstTimeDashboard,
                  box = userDetail.FirstTimeBox,
                  library = userDetail.FirstTimeLibrary,
                  item = userDetail.FirstTimeItem
              });
            context.ViewBag.firstTime = retVal;
        }
    }
}