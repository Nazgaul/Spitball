using System;
using Autofac.Extras.DynamicProxy;
using CacheManager.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;

namespace Cloudents.Infrastructure.Cache
{
    public class CacheProvider : ICacheProvider
    {
        private readonly ICacheManager<object> m_Cache;

        public CacheProvider(ICacheManager<object> cache)
        {
            m_Cache = cache;
        }

        public object Get(string key, string region)
        {
            return m_Cache.Get(key, region);
        }

        public void Set(string key, string region, object value, int expire)
        {
            var cacheItem = new CacheItem<object>(key, region, value, ExpirationMode.Sliding, TimeSpan.FromSeconds(expire));
            m_Cache.Put(cacheItem);
        }

    }
    
}
