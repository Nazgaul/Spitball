using System.Threading.Tasks;
using System;
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

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return LazyConnection.Value;
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
                m_CachePrefix = Assembly.GetExecutingAssembly().GetName().Version +

                    ConfigurationManager.AppSettings[AppKey];
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
            try
            {
                if (!m_IsCacheAvailable)
                {
                    return Task.FromResult(false);
                }
                if (!IsAppFabricCache())
                {
                    m_Cache.Insert(region + "_" + key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                        expiration);
                    return Task.FromResult(true);
                }
                var newKey = BuildCacheKey(key, region);
                var db = Connection.GetDatabase( /*region.GetHashCode()*/);
                return db.SetAsync(newKey, value, expiration);
            }
            catch (Exception ex)
            {
                Trace.TraceLog.WriteError(string.Format("AddToCacheAsync key {0} region {1}", key, region), ex);
                return Task.FromResult(false);
            }
        }

        private string BuildCacheKey(string key, string region)
        {
            var newKey = String.Format("{2}_{0}_{1}", m_CachePrefix, key, region);
            return newKey;
        }
        public bool AddToCache<T>(string key, T value, TimeSpan expiration, string region) where T : class
        {
            try
            {
                if (!m_IsCacheAvailable)
                {
                    return false;
                }
                if (!IsAppFabricCache())
                {
                    m_Cache.Insert(region + "_" + key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                        expiration);
                    return true;
                }
                var newKey = BuildCacheKey(key, region);
                var db = Connection.GetDatabase( /*region.GetHashCode()*/);
                db.Set(newKey, value, expiration);
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceLog.WriteError(string.Format("AddToCache key {0} region {1}", key, region), ex);
                return false;
            }
        }

        public bool RemoveFromCache(string region)
        {
            if (!m_IsCacheAvailable)
            {
                return false;
            }
            if (!IsAppFabricCache())
            {
                var enumerator = m_Cache.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    m_Cache.Remove(enumerator.Key.ToString());
                }
                return true;
            }
            var server = Connection.GetServer(Connection.GetEndPoints().FirstOrDefault());
            var db = Connection.GetDatabase();
            foreach (var key in server.Keys(pattern: region + "*"))
            {
                db.KeyDelete(key);
            }
            return true;
        }

        public async Task<T> GetFromCacheAsync<T>(string key, string region) where T : class
        {
            if (!m_IsCacheAvailable)
            {
                return default(T);
            }
            try
            {
                if (!IsAppFabricCache())
                    return m_Cache[region + "_" + key] as T;


                IDatabase cache = Connection.GetDatabase( /*region.GetHashCode()*/);
                var cacheKey = BuildCacheKey(key, region);

                var t = await cache.GetAsync<T>(cacheKey);
                return t;
            }
            catch (Exception ex)
            {
                Trace.TraceLog.WriteError(string.Format("GetFromCacheAsync key {0} region {1}", key, region), ex);
                return null;
            }

        }

        public T GetFromCache<T>(string key, string region) where T : class
        {
            if (!m_IsCacheAvailable)
            {
                return default(T);
            }
            try
            {
                if (!IsAppFabricCache())
                    return m_Cache[region + "_" + key] as T;


                IDatabase cache = Connection.GetDatabase(/*region.GetHashCode()*/);
                var cacheKey = BuildCacheKey(key, region);

                return cache.Get<T>(cacheKey);
            }
            catch (Exception ex)
            {
                Trace.TraceLog.WriteError(string.Format("GetFromCacheAsync key {0} region {1}", key, region), ex);
                return default(T);
            }

        }

        private bool IsAppFabricCache()
        {
            bool shouldUseCacheFromConfig;

            bool.TryParse(ConfigFetcher.Fetch("CacheUse"), out shouldUseCacheFromConfig);
            //var x = RoleEnvironment.IsAvailable && shouldUseCacheFromConfig;
            return shouldUseCacheFromConfig;
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
