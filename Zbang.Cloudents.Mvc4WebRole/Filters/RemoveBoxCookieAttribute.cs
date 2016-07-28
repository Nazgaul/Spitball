using System;
using System.Linq;
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

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EnforceLowercaseUrlAttribute : ActionFilterAttribute
    {
        private readonly bool m_Redirect;

        public EnforceLowercaseUrlAttribute(bool redirect = true)
        {
            m_Redirect = redirect;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            if (request.Url != null)
            {
                var path = request.Url.AbsolutePath;
                var containsUpperCase = path.Any(char.IsUpper);

                if (!containsUpperCase || request.HttpMethod.ToUpper() != "GET")
                    return;

                if (m_Redirect)
                    filterContext.Result = new RedirectResult(path.ToLowerInvariant(), true);
            }

            //filterContext.Result = new HttpNotFoundResult();
        }
    }
}