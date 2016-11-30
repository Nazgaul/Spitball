using System;
using System.Linq;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    //this fucks up short url
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
            bool skipLowercase = filterContext.ActionDescriptor.IsDefined(typeof(NoUrlLowercaseAttribute), inherit: true)
                                    || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(NoUrlLowercaseAttribute), inherit: true);
            if (skipLowercase) return;


            var request = filterContext.HttpContext.Request;
            if (request.Url == null) return;
            var path = System.Web.HttpUtility.UrlDecode(request.Url.AbsolutePath);
            var containsUpperCase = path.Any(char.IsUpper);

            if (!containsUpperCase || request.HttpMethod.ToUpper() != "GET")
                return;

            if (m_Redirect)
                filterContext.Result = new RedirectResult(path.ToLowerInvariant(), true);

            //filterContext.Result = new HttpNotFoundResult();
        }
    }
}