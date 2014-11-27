using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using StackExchange.Redis;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public class Cache : ICache, IDisposable
    {

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("zboxcache.redis.cache.windows.net,abortConnect=false,ssl=true,password=CxHKyXDx40vIS5EEYT0UfnVIR1OJQSPrNnXFFdi3UGI="));

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
        private const string AppKey = "DataCache";
        private readonly string m_CachePrefix;
        private readonly System.Web.Caching.Cache m_Cache;
        private readonly bool m_IsCacheAvailable;
        public Cache()
        {
            try
            {
                m_CachePrefix = Assembly.GetExecutingAssembly().GetName().Version + ConfigurationManager.AppSettings[AppKey];
                if (HttpContext.Current == null)
                {
                    m_IsCacheAvailable = false;
                    return;
                }
                m_Cache = HttpContext.Current.Cache;
                m_IsCacheAvailable = true;
            }
            catch
            {
                m_IsCacheAvailable = false;
            }

        }

        public Task<bool> AddToCacheAsync<T>(string key, T value, TimeSpan expiration, string region) where T : class
        {
            if (!m_IsCacheAvailable)
            {
                return Task.FromResult(false);
            }
            if (!IsAppFabricCache())
            {
                m_Cache.Insert(region + "_" + key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, expiration);
                return Task.FromResult(true);
            }
            var newKey = BuildCacheKey(key, region);
            var db = Connection.GetDatabase();
            return db.SetAsync(newKey, value, expiration);
        }

        private string BuildCacheKey(string key, string region)
        {
            var newKey = String.Format("{0}_{1}_{2}", m_CachePrefix, region, key);
            return newKey;
        }
        public bool AddToCache<T>(string key, T value, TimeSpan expiration, string region) where T : class
        {
            if (!m_IsCacheAvailable)
            {
                return false;
            }
            if (!IsAppFabricCache())
            {
                m_Cache.Insert(region + "_" + key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, expiration);
                return true;
            }
            var newKey = BuildCacheKey(region, key);
            var db = Connection.GetDatabase();
            db.Set(newKey, value, expiration);
            return true;
        }

        //public bool RemoveFromCache(string region, List<string> tags)
        //{
        //    if (!m_IsCacheAvailable)
        //    {
        //        return false;
        //    }
        //    if (!IsAppFabricCache())
        //    {
        //        var enumerator = m_Cache.GetEnumerator();

        //        while (enumerator.MoveNext())
        //        {
        //            m_Cache.Remove(enumerator.Key.ToString());
        //        }
        //        return true;
        //    }
        //    // try
        //    // {
        //    //var dataCache = m_DataCacheFactory.GetDefaultCache();
        //    if (tags != null)
        //    {
        //        //var elements = dataCache.GetObjectsByAnyTag(tags.Select(s => new DataCacheTag(s)), region);
        //        //foreach (var elem in elements)
        //        //{
        //        //    dataCache.Remove(elem.Key);
        //        //}
        //    }
        //    else
        //    {
        //        //dataCache.RemoveRegion(region);
        //    }

        //    return true;
        //    // }
        //    //catch (DataCacheException ex)
        //    //{
        //    //    TraceLog.WriteError(ex);
        //    //    return false;
        //    //}
        //}

        public async Task<T> GetFromCacheAsync<T>(string key, string region) where T : class
        {
            if (!m_IsCacheAvailable)
            {
                return default(T);
            }
            if (!IsAppFabricCache())
                return m_Cache[region + "_" + key] as T;


            IDatabase cache = Connection.GetDatabase();
            var cacheKey = BuildCacheKey(key, region);

            return await cache.GetAsync<T>(cacheKey);

        }

        public T GetFromCache<T>(string key, string region) where T : class
        {
            if (!m_IsCacheAvailable)
            {
                return default(T);
            }
            if (!IsAppFabricCache())
                return m_Cache[region + "_" + key] as T;


            IDatabase cache = Connection.GetDatabase();
            var cacheKey = BuildCacheKey(key, region);

            return cache.Get<T>(cacheKey);

        }

        private bool IsAppFabricCache()
        {
            bool shouldUseCacheFromConfig;

            bool.TryParse(ConfigFetcher.Fetch("CacheUse"), out shouldUseCacheFromConfig);
            return RoleEnvironment.IsAvailable && shouldUseCacheFromConfig;
        }


        public void Dispose()
        {
            //if (m_DataCacheFactory != null)
            //{
            //    m_DataCacheFactory.Dispose();
            //}
        }
    }
}
