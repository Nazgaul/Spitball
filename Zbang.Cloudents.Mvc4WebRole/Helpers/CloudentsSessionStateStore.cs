using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class CloudentsSessionStateStore : SessionStateStoreProviderBase
    {
        Microsoft.Web.DistributedCache.DistributedCacheSessionStateStoreProvider azureAppFabricCache = 
            new Microsoft.Web.DistributedCache.DistributedCacheSessionStateStoreProvider();

        public override string Description
        {
            get
            {
                return azureAppFabricCache.Description;
            }
        }
        public override string Name
        {
            get
            {
                return azureAppFabricCache.Name;
            }
        }
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            try
            {
                azureAppFabricCache.Initialize(name, config);
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
                return azureAppFabricCache.CreateNewStoreData(context, timeout);
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
                azureAppFabricCache.CreateUninitializedItem(context, id, timeout);
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
                azureAppFabricCache.Dispose();
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
                azureAppFabricCache.EndRequest(context);
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
                return azureAppFabricCache.GetItem(context, id, out locked, out lockAge, out lockId, out actions);
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
                return azureAppFabricCache.GetItemExclusive(context, id, out locked, out lockAge, out lockId, out actions);
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
                azureAppFabricCache.InitializeRequest(context);
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
                azureAppFabricCache.ReleaseItemExclusive(context, id, lockId);
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
                azureAppFabricCache.RemoveItem(context, id, lockId, item);
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
                azureAppFabricCache.ResetItemTimeout(context, id);
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
                
                azureAppFabricCache.SetAndReleaseItemExclusive(context, id, item, lockId, newItem);
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
                return azureAppFabricCache.SetItemExpireCallback(expireCallback);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("CloudentsSessionStateStore SetItemExpireCallback", ex);
                throw;
            }
        }
    }
}