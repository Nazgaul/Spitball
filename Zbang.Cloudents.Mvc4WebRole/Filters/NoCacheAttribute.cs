using System;
using System.Web;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    /// <summary>
    /// This is no cache - it should disable the back button as well from caching
    /// </summary>
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
            cache.SetCacheability(HttpCacheability.NoCache);
            cache.SetMaxAge(TimeSpan.Zero);
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            cache.SetNoStore();
        }
    }
}