using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using StackExchange.Redis;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
using TaskExtensions = Zbang.Zbox.Infrastructure.Extensions.TaskExtensions;

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

        private readonly ILogger m_Logger;

        public SystemCache(ILogger logger)
        {
            m_Logger = logger;
            //try
            //{
            var domain = Assembly.Load("Zbang.Zbox.Domain");
            var viewModel = Assembly.Load("Zbang.Zbox.ViewModel");
            var domainBuildVersion = domain.GetName().Version.Revision;
            var viewModelBuildVersion = viewModel.GetName().Version.Revision;
            m_CachePrefix = $"{domainBuildVersion}_{viewModelBuildVersion}_{ConfigurationManager.AppSettings[AppKey]}";
            m_CacheExists = m_IsRedisCacheAvailable || m_IsHttpCacheAvailable;
        }

        public Task AddToCacheAsync<T>(CacheRegions region, string key, T value, TimeSpan expiration) where T : class
        {
            if (region == null) throw new ArgumentNullException(nameof(region));
            try
            {
                if (!m_CacheExists)
                {
                    return TaskExtensions.CompletedTaskFalse;
                }
                var cacheKey = BuildCacheKey(region, key);
                if (!m_IsRedisCacheAvailable && m_IsHttpCacheAvailable)
                {
                    HttpContext.Current.Cache.Insert(cacheKey, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                        expiration);
                    return TaskExtensions.CompletedTaskTrue;
                }
                var db = Connection.GetDatabase();

                var t1 = db.StringAppendAsync(region.Region, cacheKey + ";", CommandFlags.FireAndForget);
                var t2 = db.SetAsync(cacheKey, value, expiration);
                return Task.WhenAll(t1, t2);
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex);
                return Task.FromResult(false);
            }
        }

        public void AddToCache<T>(CacheRegions region, string key, T value, TimeSpan expiration) where T : class
        {
            try
            {
                if (!m_CacheExists)
                {
                    return;
                }
                var cacheKey = BuildCacheKey(region, key);
                if (!m_IsRedisCacheAvailable && m_IsHttpCacheAvailable)
                {
                    HttpContext.Current.Cache.Insert(cacheKey, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                        expiration);
                    return;
                }
                var db = Connection.GetDatabase();

                db.StringAppend(region.Region, cacheKey + ";", CommandFlags.FireAndForget);
                db.Set(cacheKey, value, expiration);
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex);
            }
        }

        private string BuildCacheKey(CacheRegions region, string key)
        {
            if (region.SuppressVersion)
            {
                return $"{region.Region}_{key}";
            }
            var newKey = $"{region.Region}_{m_CachePrefix}_{key}";
            return newKey;
        }

        public async Task RemoveFromCacheAsync(CacheRegions region)
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
            var db = Connection.GetDatabase();
            string keys = await db.StringGetAsync(region.Region).ConfigureAwait(false);
            if (keys == null)
            {
                await db.KeyDeleteAsync(region.Region, CommandFlags.FireAndForget).ConfigureAwait(false);
                return;
            }
            var taskList = new List<Task>();
            foreach (var key in keys.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                taskList.Add(db.KeyDeleteAsync(key, CommandFlags.FireAndForget));
            }
            taskList.Add(db.KeyDeleteAsync(region.Region, CommandFlags.FireAndForget));
            await Task.WhenAll(taskList).ConfigureAwait(false);
        }

        public Task RemoveFromCacheAsyncSlowAsync(CacheRegions region)
        {
            var server = Connection.GetServer(Connection.GetEndPoints().FirstOrDefault());
            var keys = server.Keys(0, region + "*");
            var db = Connection.GetDatabase();
            var taskList = new List<Task>();
            foreach (var key in keys)
            {
                taskList.Add(db.KeyDeleteAsync(key, CommandFlags.FireAndForget));
            }
            return Task.WhenAll(taskList);
        }

        public async Task<T> GetFromCacheAsync<T>(CacheRegions region, string key) where T : class
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

                var t = await cache.GetAsync<T>(cacheKey).ConfigureAwait(false);

                if (t != default(T))
                {
                    await cache.StringAppendAsync(region.Region, cacheKey + ";", CommandFlags.FireAndForget).ConfigureAwait(false);
                }

                return t;
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex);
                return null;
            }
        }

        public T GetFromCache<T>(CacheRegions region, string key) where T : class
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
                    cache.StringAppend(region.Region, cacheKey + ";", CommandFlags.FireAndForget);
                }
                return t;
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex);
                return null;
            }
        }

        private static bool IsAppFabricCache()
        {
            bool.TryParse(ConfigFetcher.Fetch("CacheUse"), out bool shouldUseCacheFromConfig);
            return shouldUseCacheFromConfig /*&& ConfigFetcher.IsRunningOnCloud*/;
        }

        public void Dispose()
        {
            Connection.Dispose();
            GC.SuppressFinalize(this);
            //if (m_DataCacheFactory != null)
            //{
            //    m_DataCacheFactory.Dispose();
            //}
        }
    }
}
