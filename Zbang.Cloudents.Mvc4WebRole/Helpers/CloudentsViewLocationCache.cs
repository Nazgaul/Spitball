using System;
using System.Web.Caching;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class CloudentsViewLocationCache : IViewLocationCache
    {
      //  const string CacheRegion = "MVC";
       // readonly Zbox.Infrastructure.Cache.ICache m_Cache;
        //public CloudentsViewLocationCache()
        //{
        //  //  m_Cache = DependencyResolver.Current.GetService<Zbox.Infrastructure.Cache.ICache>();
        //}


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
                httpContext.Cache.Insert(key, virtualPath, null /* dependencies */, Cache.NoAbsoluteExpiration, TimeSpan.FromDays(2));
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