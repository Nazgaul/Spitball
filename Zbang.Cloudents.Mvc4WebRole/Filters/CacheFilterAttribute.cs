using System;
using System.Web;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{

    public class CacheFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets the cache duration in seconds. The default is 10 seconds.
        /// </summary>
        /// <value>The cache duration in seconds.</value>
        public int Duration
        {
            get;
            set;
        }

        public CacheFilterAttribute()
        {
            Duration = 0;
        }


        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var cache = filterContext.HttpContext.Response.Cache;
            
            if (Duration <= 0)
            {
                cache.SetCacheability(HttpCacheability.NoCache);
                cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                return;
            }

            var cacheDuration = TimeSpan.FromSeconds(Duration);

            cache.SetCacheability(HttpCacheability.Public);
            cache.SetExpires(DateTime.Now.Add(cacheDuration));
            cache.SetMaxAge(cacheDuration);
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        }
    }
}