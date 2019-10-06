using System;
using System.Collections;
using System.Collections.Generic;
using CacheManager.Core;
using Cloudents.Core.Interfaces;
using StackExchange.Redis;

namespace Cloudents.Infrastructure.Cache
{
    public class CacheProvider : ICacheProvider
    {
        private readonly ICacheManager<object> _cache;
        private readonly ILogger _logger;
        private bool _distributedEnabled = true;

        public CacheProvider(IConfigurationKeys keys, ILogger logger)
        {
            _logger = logger;

            try
            {
                var multiplexer = ConnectionMultiplexer.Connect(keys.Redis);

                multiplexer.ConnectionFailed += (sender, args) =>
                {
                    _distributedEnabled = false;

                    //Console.WriteLine("Connection failed, disabling redis...");
                };

                multiplexer.ConnectionRestored += (sender, args) =>
                {
                    _distributedEnabled = true;

                    //Console.WriteLine("Connection restored, redis is back...");
                };

                _cache = CacheFactory.Build(
                    s => s
                        .WithJsonSerializer()
                        .WithDictionaryHandle()
                        //.WithExpiration(ExpirationMode.Absolute, TimeSpan.FromSeconds(5))
                        .And
                        .WithRedisConfiguration("redis", multiplexer)
                        .WithRedisCacheHandle("redis"));

                _cache = CacheFactory.Build(settings =>
                {
                    var key = keys.Redis;
                    settings
                        .WithRedisConfiguration("redis", key)
                        .WithJsonSerializer()
                        .WithMaxRetries(1000)
                        .WithRetryTimeout(100)
                        .WithRedisBackplane("redis")
                        .WithRedisCacheHandle("redis");
                });
            }
            catch (Exception e)
            {
                _logger.Exception(e);
                
            }
        }

        //public CacheProvider(ICacheManager<object> cache, ILogger logger)
        //{
        //    _cache = cache;
        //    _logger = logger;
        //}

        public object Get(string key, string region)
        {
            if (_distributedEnabled)
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

            return null;
        }

        public T Get<T>(string key, string region)
        {
            if (_distributedEnabled)
            {
                return _cache.Get<T>(key, region);
            }

            return default;
        }

        public bool Exists(string key, string region)
        {
            if (_distributedEnabled)
                return _cache.Exists(key, region);
            return false;
        }

        public void Set(string key, string region, object value, int expire, bool slideExpiration)
        {
            if (_distributedEnabled)
            {
                var obj = ConvertEnumerableToList(value);
                if (obj == null)
                {
                    return;
                }

                var cacheItem = new CacheItem<object>(key, region, obj,
                    slideExpiration ? ExpirationMode.Sliding : ExpirationMode.Absolute,
                    TimeSpan.FromSeconds(expire));
                _cache.Put(cacheItem);
            }

            //return obj;
        }

        //public void Set<T>(string key, string region, T value, int expire, bool slideExpiration)
        //{
        //    if (_distributedEnabled)
        //    {
        //        var obj = ConvertEnumerableToList(value);
        //        if (obj == null)
        //        {
        //            return;
        //        }

        //        var cacheItem = new CacheItem<T>(key, region, value,
        //            slideExpiration ? ExpirationMode.Sliding : ExpirationMode.Absolute,
        //            TimeSpan.FromSeconds(expire));
                
        //        _cache.Put(cacheItem);
        //    }

        //    //return obj;
        //}

        public void DeleteRegion(string region)
        {
            if (_distributedEnabled)
            {
                _cache.ClearRegion(region);
            }
        }

        public void DeleteKey(string region, string key)
        {
            if (_distributedEnabled)
            {
                _cache.Remove(key, region);
            }
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
