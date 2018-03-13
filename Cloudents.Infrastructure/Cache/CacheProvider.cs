using System;
using System.Collections;
using System.Collections.Generic;
using CacheManager.Core;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Cache
{
    public class CacheProvider : ICacheProvider
    {
        private readonly ICacheManager<object> _cache;
        private readonly ILogger _logger;

        public CacheProvider(ICacheManager<object> cache, ILogger logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public object Get(string key, string region)
        {
            try
            {
                return _cache.Get(key, region);
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, new Dictionary<string, string>
                {
                    ["Service"] = nameof(Cache),
                    ["Key"] = key,
                    ["Region"] = region
                });
                _cache.Remove(key, region);
                return null;
            }
        }

        public bool Exists(string key, string region)
        {
            return _cache.Exists(key, region);
        }

        public void Set(string key, string region, object value, int expire,bool slideExpiration)
        {
            var obj = ConvertEnumerableToList(value);
            if (obj == null)
            {
                return;
            }
            var cacheItem = new CacheItem<object>(key, region, obj, slideExpiration ? ExpirationMode.Sliding : ExpirationMode.Absolute,
                TimeSpan.FromSeconds(expire));
            _cache.Put(cacheItem);
            //return obj;
        }

        /// <summary>
        /// Detect if object is IEnumerable if yes return it as IList otherwise as the same type
        /// </summary>
        /// <param name="val">The object</param>
        /// <returns></returns>
        private static object ConvertEnumerableToList(object val)
        {
            var o = val.GetType();
            var p = Array.Find(o.GetInterfaces(), t => t.IsGenericType
                          && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (p != null)
            {
                
                var t = p.GetGenericArguments()[0];
                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(t);

                var instance = (IList)Activator.CreateInstance(constructedListType);

                if (val is IEnumerable temp)
                    foreach (var obj in temp)
                    {
                        instance.Add(obj);
                    }

                return instance;
            }
            return val;
        }
    }
}
