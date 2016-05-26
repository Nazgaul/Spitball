using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using StackExchange.Redis;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public class Cache : ICache, IDisposable
    {

        private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("zboxcache.redis.cache.windows.net,abortConnect=false,ssl=true,password=CxHKyXDx40vIS5EEYT0UfnVIR1OJQSPrNnXFFdi3UGI="));

        public static ConnectionMultiplexer Connection => LazyConnection.Value;
        private const string AppKey = "DataCache";
        private readonly string m_CachePrefix;
        private readonly System.Web.Caching.Cache m_Cache;
        private readonly bool m_IsCacheAvailable;
        public Cache()
        {
            try
            {
                var domain = Assembly.Load("Zbang.Zbox.Domain");
                var viewModel = Assembly.Load("Zbang.Zbox.ViewModel");
                var domainBuildVersion = domain.GetName().Version.Revision;
                var viewModelBuildVersion = viewModel.GetName().Version.Revision;

                m_CachePrefix = $"{domainBuildVersion}_{viewModelBuildVersion}_{ConfigurationManager.AppSettings[AppKey]}";
               
                if (HttpContext.Current == null)
                {
                    m_IsCacheAvailable = false;
                    return;
                }
                m_Cache = HttpContext.Current.Cache;
               // m_IsCacheAvailable = true;
                m_IsCacheAvailable = IsAppFabricCache();
            }
            catch
            {
                m_IsCacheAvailable = false;
            }

        }

        public Task<bool> AddToCacheAsync<T>(string region, string key, T value, TimeSpan expiration) where T : class
        {
            try
            {
                if (!m_IsCacheAvailable)
                {
                    return Task.FromResult(false);
                }
                var cacheKey = BuildCacheKey(region, key);
                if (!IsAppFabricCache())
                {
                    m_Cache.Insert(cacheKey, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                        expiration);
                    return Task.FromResult(true);
                }
                var db = Connection.GetDatabase( /*region.GetHashCode()*/);
                return db.SetAsync(cacheKey, value, expiration);
            }
            catch (Exception ex)
            {
                Trace.TraceLog.WriteError($"AddToCacheAsync key {key}", ex);
                return Task.FromResult(false);
            }
        }

        private string BuildCacheKey(string region, string key)
        {
            var newKey = $"{region}_{m_CachePrefix}_{key}";
            return newKey;
        }
        


        public async Task RemoveFromCacheAsync(string region)
        {
            if (!m_IsCacheAvailable)
            {
                return;
            }
            if (!IsAppFabricCache())
            {
                var enumerator = m_Cache.GetEnumerator();

                while (enumerator.MoveNext())
                {

                    m_Cache.Remove(enumerator.Key.ToString());
                }
                return;
            }
            var server = Connection.GetServer(Connection.GetEndPoints().FirstOrDefault());
            var db = Connection.GetDatabase();
            var taskList = new List<Task>();
            foreach (var key in server.Keys(pattern: region + "*"))
            {
                taskList.Add(db.KeyDeleteAsync(key));
            }
            await Task.WhenAll(taskList);

        }

        public async Task<T> GetFromCacheAsync<T>(string region, string key) where T : class
        {
            if (!m_IsCacheAvailable)
            {
                return default(T);
            }
            try
            {
                var cacheKey = BuildCacheKey(region, key);
                if (!IsAppFabricCache())
                    return m_Cache[cacheKey] as T;


                IDatabase cache = Connection.GetDatabase( /*region.GetHashCode()*/);

                var t = await cache.GetAsync<T>(cacheKey);
                return t;
            }
            catch (Exception ex)
            {
                Trace.TraceLog.WriteError($"GetFromCacheAsync key {key}", ex);
                return null;
            }

        }

       

        private static bool IsAppFabricCache()
        {
            bool shouldUseCacheFromConfig;

            bool.TryParse(ConfigFetcher.Fetch("CacheUse"), out shouldUseCacheFromConfig);
            return shouldUseCacheFromConfig;
        }


        public void Dispose()
        {
            Connection.Dispose();
            //if (m_DataCacheFactory != null)
            //{
            //    m_DataCacheFactory.Dispose();
            //}
        }
    }
}
