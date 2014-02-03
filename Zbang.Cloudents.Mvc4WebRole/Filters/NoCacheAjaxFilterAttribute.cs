using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    /// <summary>
    /// Global Filter to pervent ajax request from being cached by the browser
    /// </summary>
    public class NoCacheAjaxFilterAttribute : ActionFilterAttribute
    {
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
                cache.SetCacheability(HttpCacheability.NoCache);
                cache.SetNoStore();
                cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}