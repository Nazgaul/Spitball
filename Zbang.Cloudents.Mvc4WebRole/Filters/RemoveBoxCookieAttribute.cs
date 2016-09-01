using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Helpers;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class RemoveBoxCookieAttribute : ActionFilterAttribute
    {
        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
            
        //}
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookieHelper = new CookieHelper(filterContext.HttpContext);
            cookieHelper.RemoveCookie(BoxPermissionAttribute.Permission.CookieName);
            base.OnActionExecuting(filterContext);
        }
    }
}