using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Helpers;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class RemoveBoxCookieAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookieHelper = new CookieHelper(filterContext.HttpContext);
            cookieHelper.RemoveCookie("p");
            base.OnActionExecuting(filterContext);
        }
    }
}