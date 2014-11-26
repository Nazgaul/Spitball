using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using Microsoft.Web.Redis;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class CloudentsOutsputCacheProvider : System.Web.Caching.OutputCacheProvider
    {
        readonly Microsoft.Web.Redis.RedisOutputCacheProvider m_Cache
            = new Microsoft.Web.Redis.RedisOutputCacheProvider();

        readonly string m_CachePrefix = Assembly.GetExecutingAssembly().GetName().Version
            + ConfigurationManager.AppSettings["DataCache"];

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            try
            {
                m_Cache.Initialize(name, config);
                base.Initialize(name, config);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsOutsputCacheProvider Initialize", ex);
            }

        }
        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            try
            {
                return m_Cache.Add(m_CachePrefix + key, entry, utcExpiry);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsOutsputCacheProvider Add", ex);
                return m_Cache;
            }

        }

        public override object Get(string key)
        {
            try
            {
                return m_Cache.Get(m_CachePrefix + key);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsOutsputCacheProvider Get", ex);
                return null;
            }
        }

        public override void Remove(string key)
        {
            try
            {
                m_Cache.Remove(m_CachePrefix + key);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsOutsputCacheProvider Remove", ex);
            }
        }

        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            try
            {
                m_Cache.Set(m_CachePrefix + key, entry, utcExpiry);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsOutsputCacheProvider Set", ex);
            }
        }
    }
}