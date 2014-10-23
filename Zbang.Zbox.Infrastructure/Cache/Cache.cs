using System.Diagnostics;
using Microsoft.ApplicationServer.Caching;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public class Cache : ICache , IDisposable
    {
        private const string AppKey = "DataCache";
        private readonly string m_CachePrefix;
        private readonly System.Web.Caching.Cache m_Cache;
        private readonly bool m_IsCacheAvailable;
        private readonly DataCacheFactory m_DataCacheFactory;
        public Cache()
        {
            try
            {
                m_CachePrefix = Assembly.GetExecutingAssembly().GetName().Version + ConfigurationManager.AppSettings[AppKey];
                DataCacheClientLogManager.ChangeLogLevel(TraceLevel.Off);
                DataCacheClientLogManager.SetSink(DataCacheTraceSink.DiagnosticSink, TraceLevel.Off);
                if (HttpContext.Current == null)
                {
                    m_IsCacheAvailable = false;
                    return;
                }
                m_Cache = HttpContext.Current.Cache;
                m_DataCacheFactory = new DataCacheFactory();
                m_IsCacheAvailable = true;
            }
            catch
            {
                m_IsCacheAvailable = false;
            }

        }
        public bool AddToCache(string key, object value, TimeSpan expiration, string region)
        {
            return AddToCache(key, value, expiration, region, null);
        }

        public bool AddToCache(string key, object value, TimeSpan expiration, string region, List<string> tags)
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
            try
            {
                var keyWithPrefix = m_CachePrefix + key;
                var dataCache = m_DataCacheFactory.GetDefaultCache();

                dataCache.CreateRegion(region);
                if (tags == null)
                {
                    dataCache.Put(keyWithPrefix, value, expiration, region);
                    return true;
                }
                if (tags.Count == 0)
                {
                    dataCache.Put(keyWithPrefix, value, expiration, region);
                    return true;
                }
                dataCache.Put(keyWithPrefix, value, expiration, tags.Select(s => new DataCacheTag(s)), region);

                return true;
            }
            catch (DataCacheException ex)
            {
                TraceLog.WriteError(string.Format("key: {0}, region: {1}", key, region), ex);
                return false;
            }
        }

        public bool RemoveFromCache(string region, List<string> tags)
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
            try
            {
                var dataCache = m_DataCacheFactory.GetDefaultCache();
                if (tags != null)
                {
                    var elements = dataCache.GetObjectsByAnyTag(tags.Select(s => new DataCacheTag(s)), region);
                    foreach (var elem in elements)
                    {
                        dataCache.Remove(elem.Key);
                    }
                }
                else
                {
                    dataCache.RemoveRegion(region);
                }

                return true;
            }
            catch (DataCacheException ex)
            {
                TraceLog.WriteError(ex);
                return false;
            }
        }

        public object GetFromCache(string key, string region)
        {
            if (!m_IsCacheAvailable)
            {
                return null;
            }
            if (!IsAppFabricCache())
                return m_Cache[region + "_" + key];
            try
            {

                var keyWithPrefix = m_CachePrefix + key;
                TraceLog.WriteInfo("Try to get " + keyWithPrefix);
                var dataCache = m_DataCacheFactory.GetDefaultCache();

                var obj = dataCache.Get(keyWithPrefix);
                TraceLog.WriteInfo((obj != null).ToString());
                return obj;

            }
            catch (DataCacheException ex)
            {
                TraceLog.WriteInfo(ex);
                return null;
            }

        }

        private bool IsAppFabricCache()
        {
            bool shouldUseCacheFromConfig;

            bool.TryParse(ConfigFetcher.Fetch("CacheUse"), out shouldUseCacheFromConfig);
            return RoleEnvironment.IsAvailable && shouldUseCacheFromConfig;
        }


        public void Dispose()
        {
            if (m_DataCacheFactory != null)
            {
                m_DataCacheFactory.Dispose();
            }
        }
    }
}
