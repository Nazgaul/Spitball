using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Cloudents.Mobile.Filters
{
    public class RedirectToDesktopSiteAttribute : ActionFilterAttribute
    {
       
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var urlToRedirect = ConfigFetcher.Fetch("DesktopWebSite");
            var url = filterContext.HttpContext.Request.Url;
            if (url != null)
            {
                urlToRedirect = urlToRedirect + url.PathAndQuery;
            }
            filterContext.Result = new RedirectResult(urlToRedirect);
        }
    }
}