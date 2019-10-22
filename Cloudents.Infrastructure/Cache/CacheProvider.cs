using System;
using System.Collections;
using System.Collections.Generic;
using CacheManager.Core;
using Cloudents.Core.Interfaces;
using StackExchange.Redis;

namespace Cloudents.Infrastructure.Cache
{
    public sealed class CacheProvider : ICacheProvider, IDisposable
    {
        private ICacheManager<object> _distributedCache;
        private ICacheManager<object> _inMemory;
        private readonly IConfigurationKeys _keys;
        private readonly ILogger _logger;
        private bool _distributedEnabled = true;
        readonly ConnectionMultiplexer _multiplexer;

        private bool _failedInit = false;

        public CacheProvider(IConfigurationKeys keys, ILogger logger)
        {
            _keys = keys;
            _logger = logger;
        
            _multiplexer = ConnectionMultiplexer.Connect(keys.Redis);

            _multiplexer.ConnectionFailed += (sender, args) =>
            {
                _distributedEnabled = false;

                //Console.WriteLine("Connection failed, disabling redis...");
            };

            _multiplexer.ConnectionRestored += (sender, args) =>
            {
                _distributedEnabled = true;

                //Console.WriteLine("Connection restored, redis is back...");
            };
            _inMemory = CacheFactory.Build(
                s => s
                    .WithDictionaryHandle());
            TryReconnect(keys);
        }

        private void TryReconnect(IConfigurationKeys keys)
        {
            //try
            //{
            //sb-dev.redis.cache.windows.net:6380,password=SggEokcdmQnaS5RFkkl5js4nV0LLXjvXEypeEEFCAo8=,ssl=True,abortConnect=False
            //sb-dev2.redis.cache.windows.net:6380,password=SggEokcdmQnaS5RFkkl5js4nV0LLXjvXEypeEEFCAo8=,ssl=True,abortConnect=False
           
            try
            {
                _distributedCache = CacheFactory.Build(
                    s => s
                        .WithJsonSerializer()
                        .WithDictionaryHandle()
                        .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromSeconds(5))
                        .And
                        .WithRedisConfiguration("redis", _multiplexer)
                        .WithRedisCacheHandle("redis"));
                _failedInit = false;
            }
            catch (InvalidOperationException e)
            {
                _failedInit = true;
                _distributedEnabled = false;
                _logger.Exception(e);
            }

          
            // .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(5)));

            //_cache = CacheFactory.Build(settings =>
            //{
            //    var key = keys.Redis;
            //    settings
            //        .WithRedisConfiguration("redis", key)
            //        .WithJsonSerializer()
            //        .WithMaxRetries(1000)
            //        .WithRetryTimeout(100)
            //        .WithRedisBackplane("redis")
            //        .WithRedisCacheHandle("redis");
            //});
            //}
            //catch (Exception e)
            //{
            // _distributedEnabled = false;
            //_logger.Exception(e);
            //}
        }

        //public CacheProvider(ICacheManager<object> cache, ILogger logger)
        //{
        //    _cache = cache;
        //    _logger = logger;
        //}

        private ICacheManager<object> cache
        {
            get
            {
                if (_failedInit)
                {
                    TryReconnect(_keys);
                }
                if (_distributedEnabled)
                {
                    Console.WriteLine("distributed");
                    return _distributedCache;
                }

                Console.WriteLine("Memory");
                return _inMemory;
            }
        }


        public object Get(string key, string region)
        {
            try
            {
                return cache.Get(key, region);
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, new Dictionary<string, string>
                {
                    ["Service"] = nameof(Cache),
                    ["Key"] = key,
                    ["Region"] = region
                });
            }

            return null;
        }

        public bool Exists(string key, string region)
        {

            return cache.Exists(key, region);

        }

        public void Set(string key, string region, object value, int expire, bool slideExpiration)
        {

            try
            {
                var obj = ConvertEnumerableToList(value);
                if (obj == null)
                {
                    return;
                }

                var cacheItem = new CacheItem<object>(key, region, obj,
                    slideExpiration ? ExpirationMode.Sliding : ExpirationMode.Absolute,
                    TimeSpan.FromSeconds(expire));
                cache.Put(cacheItem);
            }
            catch (Exception e)
            {
                _logger.Exception(e, new Dictionary<string, string>
                {
                    ["Service"] = nameof(Cache),
                    ["Key"] = key,
                    ["Region"] = region,
                    ["Value"] = value.ToString()
                });
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
            // if (_distributedEnabled)
            //{
            cache.ClearRegion(region);
            //}
        }

        public void DeleteKey(string region, string key)
        {
            // if (_distributedEnabled)
            // {
            cache.Remove(key, region);
            // }
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

        public void Dispose()
        {
            _distributedCache?.Dispose();
            _inMemory?.Dispose();
            _multiplexer?.Dispose();
        }
    }
}
