using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class CloudentsViewLocationCache : IViewLocationCache
    {
        const string CacheRegion = "MVC";
        Zbang.Zbox.Infrastructure.Cache.ICache m_Cache;
        public CloudentsViewLocationCache()
        {
            m_Cache = DependencyResolver.Current.GetService<Zbang.Zbox.Infrastructure.Cache.ICache>();
        }


        public string GetViewLocation(System.Web.HttpContextBase httpContext, string key)
        {
            try
            {
                var localCacheValue = (string)httpContext.Cache[key];
                if (localCacheValue == null)
                {
                    localCacheValue = (string)m_Cache.GetFromCache(key, CacheRegion);
                }
                return localCacheValue;
            }
            catch (Exception ex)
            {

                TraceLog.WriteError("GetViewLocation", ex);
                return (string)httpContext.Cache[key];
            }
        }

        public void InsertViewLocation(System.Web.HttpContextBase httpContext, string key, string virtualPath)
        {
            try
            {
                m_Cache.AddToCache(key, virtualPath, TimeSpan.FromDays(2), CacheRegion);
                httpContext.Cache.Insert(key, virtualPath, null /* dependencies */, Cache.NoAbsoluteExpiration, TimeSpan.FromDays(2));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("InsertViewLocation", ex);
                httpContext.Cache.Insert(key, virtualPath, null /* dependencies */, Cache.NoAbsoluteExpiration, TimeSpan.FromDays(2));
            }

        }
    }
}