using System.Web.Mvc;

namespace Zbang.Cloudents.SiteExtension
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