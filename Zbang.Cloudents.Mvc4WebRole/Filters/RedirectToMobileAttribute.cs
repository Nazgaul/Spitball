using System.Web;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Extensions;


namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class RedirectToMobileAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.Browser.IsMobileDevice) return;
            var urlToRedirect = ConfigFetcher.Fetch("MobileWebSite");
            var url = filterContext.HttpContext.Request.Url;
            var path = string.Empty;
            if (url != null)
            {
                path = url.PathAndQuery;
            }
            urlToRedirect = VirtualPathUtility.RemoveTrailingSlash(urlToRedirect);
            urlToRedirect = urlToRedirect + path;
            filterContext.Result = new RedirectResult(urlToRedirect, true);
        }
    }

    public class RedirectToWWW : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var serverDomain = filterContext.HttpContext.Request.Url.Host;
            if (serverDomain.StartsWith("ono"))
            {
                filterContext.Result = new RedirectResult("https://www.Spitball.co" + filterContext.HttpContext.Request.Url.PathAndQuery, true);
            }
           
            base.OnActionExecuting(filterContext);
        }

         
    }
}