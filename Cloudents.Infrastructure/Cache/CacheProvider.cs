using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public object Set(string key, string region, object value, int expire)
        {
            var obj = ConvertEnumerableToList(value);
            if (obj == null)
            {
                return value;
            }
            var cacheItem = new CacheItem<object>(key, region, obj, ExpirationMode.Sliding,
                TimeSpan.FromSeconds(expire));
            _cache.Put(cacheItem);
            return obj;
        }

        /// <summary>
        /// Detect if object is IEnumerable if yes return it as IList otherwise as the same type
        /// </summary>
        /// <param name="val">The object</param>
        /// <returns></returns>
        private static object ConvertEnumerableToList(object val)
        {
            var o = val.GetType();
            var p = o.GetInterfaces()
                .FirstOrDefault(t => t.IsGenericType
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
