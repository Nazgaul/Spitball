using CacheManager.Core;
using Cloudents.Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cloudents.Infrastructure.Cache
{
    public sealed class CacheProvider : ICacheProvider, IDisposable
    {
        private ICacheManager<object> _distributedCache;
        private readonly ICacheManager<object> _inMemory;
        private readonly ILogger _logger;
        private bool _distributedEnabled = true;
        readonly ConnectionMultiplexer _multiplexer;

        private bool _failedInit;

        public CacheProvider(IConfigurationKeys keys, ILogger logger)
        {
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
            TryReconnect();
        }

        private void TryReconnect()
        {

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
        }
      

        private ICacheManager<object> Cache
        {
            get
            {
                if (_failedInit)
                {
                    TryReconnect();
                }
                if (_distributedEnabled)
                {
                    return _distributedCache;
                }

                return _inMemory;
            }
        }


        public object Get(string key, string region)
        {
            try
            {
                return Cache.Get(key, region);
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, new Dictionary<string, string>
                {
                    ["Service"] = nameof(Infrastructure.Cache),
                    ["Key"] = key,
                    ["Region"] = region
                });
            }

            return null;
        }

        public T Get<T>(string key, string region)
        {
            try
            {
                return Cache.Get<T>(key, region);
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, new Dictionary<string, string>
                {
                    ["Service"] = nameof(Infrastructure.Cache),
                    ["Key"] = key,
                    ["Region"] = region
                });
            }

            return default;
        }

        public void Set(string key, string region, object value, TimeSpan expire, bool slideExpiration)
        {
            if (value is null)
            {
                return;
            }
            try
            {
                var obj = ConvertEnumerableToList(value);
                if (obj == null)
                {
                    return;
                }

                if (!_distributedEnabled && expire > TimeSpan.FromMinutes(5))
                {
                    expire = TimeSpan.FromMinutes(5);
                }

                var cacheItem = new CacheItem<object>(key, region, obj,
                    slideExpiration ? ExpirationMode.Sliding : ExpirationMode.Absolute,
                    expire);
                Cache.Put(cacheItem);
            }
            catch (Exception e)
            {
                _logger.Exception(e, new Dictionary<string, string>
                {
                    ["Service"] = nameof(Infrastructure.Cache),
                    ["Key"] = key,
                    ["Region"] = region,
                    ["Value"] = value.ToString()
                });
            }
        }

        public bool Exists(string key, string region)
        {

            return Cache.Exists(key, region);

        }

        public void Set(string key, string region, object value, int expire, bool slideExpiration)
        {
            Set(key, region, value, TimeSpan.FromSeconds(expire), slideExpiration);


            //return obj;
        }
        

        public void DeleteRegion(string region)
        {
         
            Cache.ClearRegion(region);
           
        }

        public void DeleteKey(string region, string key)
        {
          
            Cache.Remove(key, region);
           
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
