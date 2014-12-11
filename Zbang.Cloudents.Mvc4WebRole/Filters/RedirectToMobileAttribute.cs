using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class RedirectToMobileAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var urlToRedirect = ConfigFetcher.Fetch("MobileWebSite");
            var url = filterContext.HttpContext.Request.Url;
            urlToRedirect = urlToRedirect + url.PathAndQuery;

            filterContext.Result = new RedirectResult(urlToRedirect);
        }
    }
}