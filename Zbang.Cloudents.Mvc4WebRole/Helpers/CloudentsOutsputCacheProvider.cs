using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class CloudentsOutsputCacheProvider : System.Web.Caching.OutputCacheProvider
    {
        Microsoft.Web.DistributedCache.DistributedCacheOutputCacheProvider m_Cache = new Microsoft.Web.DistributedCache.DistributedCacheOutputCacheProvider();

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
                return m_Cache.Add(key, entry, utcExpiry);
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
                return m_Cache.Get(key);
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
                m_Cache.Remove(key);
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
                m_Cache.Set(key, entry, utcExpiry);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsOutsputCacheProvider Set", ex);
            }
        }
    }
}