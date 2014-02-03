using System;
using System.Web;
using System.Web.Mvc;

namespace Zbang.Zbox.Mvc3WebRole.Attributes
{
    //public class CacheFilterAttribute : ActionFilterAttribute
    //{
    //    /// <summary>
    //    /// Gets or sets the cache duration in seconds. The default is 10 seconds.
    //    /// </summary>
    //    /// <value>The cache duration in seconds.</value>
    //    public int Duration
    //    {
    //        get;
    //        set;
    //    }

    //    public CacheFilterAttribute()
    //    {
    //        Duration = 10;
    //    }

    //    public override void OnActionExecuted(ActionExecutedContext filterContext)
    //    {
    //        if (Duration <= 0) return;

    //        var cache = filterContext.HttpContext.Response.Cache;
    //        var cacheDuration = TimeSpan.FromSeconds(Duration);
    //        if (filterContext.HttpContext.User.Identity.IsAuthenticated)
    //        {
    //            cache.SetCacheability(HttpCacheability.Private);
    //        }
    //        else
    //        {
    //            cache.SetCacheability(HttpCacheability.Public);
    //        }
    //        cache.SetExpires(DateTime.Now.Add(cacheDuration));
    //        cache.SetMaxAge(cacheDuration);
    //        cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
    //    }
    //}

    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
            cache.SetCacheability(HttpCacheability.NoCache);
            cache.SetNoStore();
            cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
        }
        
    }

    //public class CacheNoStoreAttribute : ActionFilterAttribute
    //{
    //    public override void OnActionExecuted(ActionExecutedContext filterContext)
    //    {
    //        HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
    //        cache.SetNoStore();
    //        cache.SetCacheability(HttpCacheability.NoCache);
    //        cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
    //    }
    //}
}