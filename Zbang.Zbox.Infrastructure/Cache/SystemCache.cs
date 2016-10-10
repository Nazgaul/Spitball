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
    public class SystemCache : ICache, IDisposable
    {

        private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("zboxcache.redis.cache.windows.net,abortConnect=false,allowAdmin=true,ssl=true,password=CxHKyXDx40vIS5EEYT0UfnVIR1OJQSPrNnXFFdi3UGI="));

        public static ConnectionMultiplexer Connection => LazyConnection.Value;
        private const string AppKey = "DataCache";
        private readonly string m_CachePrefix;

        private readonly bool m_IsRedisCacheAvailable = IsAppFabricCache();
        private readonly bool m_IsHttpCacheAvailable = HttpContext.Current != null;
        private readonly bool m_CacheExists;


        //private readonly System.Web.Caching.Cache m_Cache;
        // private readonly bool m_IsCacheAvailable;
        public SystemCache()
        {
            //try
            //{
            var domain = Assembly.Load("Zbang.Zbox.Domain");
            var viewModel = Assembly.Load("Zbang.Zbox.ViewModel");
            var domainBuildVersion = domain.GetName().Version.Revision;
            var viewModelBuildVersion = viewModel.GetName().Version.Revision;
            m_CachePrefix = $"{domainBuildVersion}_{viewModelBuildVersion}_{ConfigurationManager.AppSettings[AppKey]}";
            m_CacheExists = m_IsRedisCacheAvailable || m_IsHttpCacheAvailable;
        }

        public Task AddToCacheAsync<T>(string region, string key, T value, TimeSpan expiration) where T : class
        {
            try
            {
                if (!m_CacheExists)
                {
                    return Extensions.TaskExtensions.CompletedTaskFalse;
                }
                var cacheKey = BuildCacheKey(region, key);
                if (!m_IsRedisCacheAvailable && m_IsHttpCacheAvailable)
                {
                    HttpContext.Current.Cache.Insert(cacheKey, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                        expiration);
                    return Extensions.TaskExtensions.CompletedTaskTrue;
                }
                var db = Connection.GetDatabase();
                var t1 = db.StringAppendAsync(region, cacheKey + ";", CommandFlags.FireAndForget);
                var t2 = db.SetAsync(cacheKey, value, expiration);
                return Task.WhenAll(t1, t2);
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
            if (!m_CacheExists)
            {
                return;
            }
            if (!m_IsRedisCacheAvailable && m_IsHttpCacheAvailable)
            {
                var enumerator = HttpContext.Current.Cache.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    HttpContext.Current.Cache.Remove(enumerator.Key.ToString());
                }
                return;
            }
            //var server = Connection.GetServer(Connection.GetEndPoints().FirstOrDefault());
            var db = Connection.GetDatabase();
            string keys = await db.StringGetAsync(region);
            if (keys == null)
            {
                await db.KeyDeleteAsync(region, CommandFlags.FireAndForget);
                return;
            }
            var taskList = new List<Task>();
            foreach (var key in keys.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                taskList.Add(db.KeyDeleteAsync(key, CommandFlags.FireAndForget));
            }
            taskList.Add(db.KeyDeleteAsync(region, CommandFlags.FireAndForget));
            await Task.WhenAll(taskList);

        }

        public async Task RemoveFromCacheAsyncSlowAsync(string region)
        {
            var server = Connection.GetServer(Connection.GetEndPoints().FirstOrDefault());
            var keys = server.Keys(0, region + "*");
            var db = Connection.GetDatabase();
            var taskList = new List<Task>();
            foreach (var key in keys)
            {
                taskList.Add(db.KeyDeleteAsync(key, CommandFlags.FireAndForget));
            }
            await Task.WhenAll(taskList);
        }

        public async Task<T> GetFromCacheAsync<T>(string region, string key) where T : class
        {
            if (!m_CacheExists)
            {
                return default(T);
            }
            try
            {
                var cacheKey = BuildCacheKey(region, key);
                if (!m_IsRedisCacheAvailable && m_IsHttpCacheAvailable)
                    return HttpContext.Current.Cache[cacheKey] as T;


                var cache = Connection.GetDatabase();

                var t = await cache.GetAsync<T>(cacheKey);
                if (t != default(T))
                {
                    await cache.StringAppendAsync(region, cacheKey + ";", CommandFlags.FireAndForget);
                }
                return t;
            }
            catch (Exception ex)
            {
                Trace.TraceLog.WriteError($"GetFromCacheAsync key {key}", ex);
                return null;
            }

        }


        public T GetFromCache<T>(string region, string key) where T : class
        {
            if (!m_CacheExists)
            {
                return default(T);
            }
            try
            {
                var cacheKey = BuildCacheKey(region, key);
                if (!m_IsRedisCacheAvailable && m_IsHttpCacheAvailable)
                    return HttpContext.Current.Cache[cacheKey] as T;


                var cache = Connection.GetDatabase();

                var t = cache.Get<T>(cacheKey);
                if (t != default(T))
                {
                    cache.StringAppend(region, cacheKey + ";", CommandFlags.FireAndForget);
                }
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
            return shouldUseCacheFromConfig /*&& ConfigFetcher.IsRunningOnCloud*/;
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
