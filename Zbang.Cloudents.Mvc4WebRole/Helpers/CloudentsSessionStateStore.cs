using System;
using System.Web;
using System.Web.SessionState;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class CloudentsSessionStateStore : SessionStateStoreProviderBase
    {
        readonly Microsoft.Web.Redis.RedisSessionStateProvider m_AzureAppFabricCache =
            new Microsoft.Web.Redis.RedisSessionStateProvider();

        public override string Description
        {
            get
            {
                return m_AzureAppFabricCache.Description;
            }
        }
        public override string Name
        {
            get
            {
                return m_AzureAppFabricCache.Name;
            }
        }
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            try
            {
                m_AzureAppFabricCache.Initialize(name, config);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsSessionStateStore Initialize", ex);
            }
        }
        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            try
            {
                return m_AzureAppFabricCache.CreateNewStoreData(context, timeout);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsSessionStateStore CreateNewStoreData", ex);
                throw;
            }
        }

        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            try
            {
                m_AzureAppFabricCache.CreateUninitializedItem(context, id, timeout);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsSessionStateStore CreateUninitializedItem", ex);
            }
        }

        public override void Dispose()
        {
            try
            {
                m_AzureAppFabricCache.Dispose();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsSessionStateStore Dispose", ex);
            }
        }

        public override void EndRequest(HttpContext context)
        {
            try
            {
                m_AzureAppFabricCache.EndRequest(context);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsSessionStateStore EndRequest", ex);
            }
        }

        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            try
            {
                return m_AzureAppFabricCache.GetItem(context, id, out locked, out lockAge, out lockId, out actions);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsSessionStateStore GetItem", ex);
                throw;
            }
        }

        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            try
            {
                return m_AzureAppFabricCache.GetItemExclusive(context, id, out locked, out lockAge, out lockId, out actions);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsSessionStateStore GetItemExclusive", ex);
                throw;
            }
        }

        public override void InitializeRequest(HttpContext context)
        {
            try
            {
                m_AzureAppFabricCache.InitializeRequest(context);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsSessionStateStore InitializeRequest", ex);
                
            }
        }

        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            try
            {
                m_AzureAppFabricCache.ReleaseItemExclusive(context, id, lockId);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsSessionStateStore ReleaseItemExclusive", ex);
            }
        }

        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            try
            {
                m_AzureAppFabricCache.RemoveItem(context, id, lockId, item);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsSessionStateStore RemoveItem", ex);
            }
        }

        public override void ResetItemTimeout(HttpContext context, string id)
        {
            try
            {
                m_AzureAppFabricCache.ResetItemTimeout(context, id);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsSessionStateStore ResetItemTimeout", ex);
            }
        }

        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            try
            {
                
                m_AzureAppFabricCache.SetAndReleaseItemExclusive(context, id, item, lockId, newItem);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsSessionStateStore SetAndReleaseItemExclusive", ex);
            }
        }

        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            try
            {
                return m_AzureAppFabricCache.SetItemExpireCallback(expireCallback);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsSessionStateStore SetItemExpireCallback", ex);
                throw;
            }
        }
    }
}