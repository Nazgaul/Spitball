using System;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    /// <summary>
    /// Global Filter to pervent ajax request from being cached by the browser
    /// </summary>
    public class NoCacheAjaxFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            bool skipNoCache = filterContext.ActionDescriptor.IsDefined(typeof(DonutOutputCacheAttribute), inherit: true)
                                    || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(DonutOutputCacheAttribute), inherit: true);
            if (skipNoCache)
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
            cache.SetCacheability(HttpCacheability.NoCache);
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            cache.SetNoStore();
            base.OnActionExecuting(filterContext);
        }
    }
}