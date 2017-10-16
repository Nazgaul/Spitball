using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            try
            {
                return m_Cache.Get(key, region);
            }
            catch
            {
                m_Cache.Remove(key, region);
                return null;
            }
        }

        public void Set(string key, string region, object value, int expire)
        {
            if (value == null)
            {
                return;
            }
            //if (value is IEnumerable iEnumerable)
            //{
                
               // Activator.CreateInstance(typeof(List<>).MakeGenericType(TypeObjectOfT), paginatedListObject);
               // value = iEnumerable.ToList();
           // }
            var cacheItem = new CacheItem<object>(key, region, value, ExpirationMode.Sliding,
                TimeSpan.FromSeconds(expire));
            m_Cache.Put(cacheItem);
        }
    }
}
