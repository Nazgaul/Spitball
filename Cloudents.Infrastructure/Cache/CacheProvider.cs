using System;
using CacheManager.Core;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Cache
{
    public class CacheProvider : ICacheProvider
    {
        private readonly ICacheManager<object> _cache;

        public CacheProvider(ICacheManager<object> cache)
        {
            _cache = cache;
        }

        public object Get(string key, string region)
        {
            try
            {
                return _cache.Get(key, region);
            }
            catch (Exception ex)
            {
                _cache.Remove(key, region);
                return null;
            }
        }

        public void Set(string key, string region, object value, int expire)
        {
            if (value == null)
            {
                return;
            }
            var cacheItem = new CacheItem<object>(key, region, value, ExpirationMode.Sliding,
                TimeSpan.FromSeconds(expire));
            _cache.Put(cacheItem);
        }
    }
}
