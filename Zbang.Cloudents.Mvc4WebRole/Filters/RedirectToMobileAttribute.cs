using System.Web.Mvc;
using System.Web.Routing;


namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class RedirectToMobileAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.Browser.IsMobileDevice) return;
            //var urlToRedirect = ConfigFetcher.Fetch("MobileWebSite");
            var url = filterContext.HttpContext.Request.Url;
            var path = string.Empty;
            if (url != null)
            {
                path = url.PathAndQuery;
            }
            //urlToRedirect = VirtualPathUtility.RemoveTrailingSlash(urlToRedirect);
            //urlToRedirect = urlToRedirect + path;
            filterContext.Result = new RedirectToRouteResult("Mobile", new RouteValueDictionary(new { returnUrl = path }));
            //filterContext.Result = filterContext.Controller new Redirect(urlToRedirect, true);
        }
    }

    public class RedirectFromCloudentsToSpitballAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
             var serverDomain = filterContext.HttpContext.Request.UrlReferrer.Host;
            if (serverDomain.ToLower().Contains("cloudents"))
            {
                filterContext.Controller.ViewBag.fromCloudents = "moveToSpitBall";
            }
            base.OnResultExecuting(filterContext);
        }
    }

    //public class RedirectToWWW : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        var serverDomain = filterContext.HttpContext.Request.Url.Host;
    //        if (serverDomain.StartsWith("ono"))
    //        {
    //            filterContext.Result = new RedirectResult("https://www.Spitball.co" + filterContext.HttpContext.Request.Url.PathAndQuery, true);
    //        }

    //        base.OnActionExecuting(filterContext);
    //    }


    //}
}