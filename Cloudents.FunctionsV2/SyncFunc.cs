using Autofac;
using Cloudents.FunctionsV2.Sync;
using Cloudents.Query.Sync;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class SyncFunc
    {

        private const string SearchSyncName = "SearchSync";
        //private const string CreateIndexFunctionName = "SearchSyncCreateIndex";
        private const string DoSyncFunctionName = "SearchSyncSync";
        private const string GetSyncStatusFunctionName = "SearchSyncGetProgress";
        private const string SetSyncStatusFunctionName = "SearchSyncSetProgress";


        internal static async Task StartSearchSync(DurableOrchestrationClient starter,
            ILogger log, SyncType syncType)

        {
            var model = new SearchSyncInput(syncType);
            var existingInstance = await starter.GetStatusAsync(model.InstanceId);
            if (existingInstance == null)
            {
                log.LogInformation($"start new instance of {syncType}");
                await starter.StartNewAsync(SearchSyncName, model.InstanceId, model);
                return;

            }
            if (existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Failed)
            {
                log.LogInformation($"terminate existing instance");
                await starter.TerminateAsync(model.InstanceId, "the status failed");
            }

            if (existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            {
                if (existingInstance.LastUpdatedTime < DateTime.UtcNow.AddHours(-1))
                {
                    log.LogError($"issue with {syncType}");
                    await starter.TerminateAsync(model.InstanceId, $"issue with {syncType}");
                }
                else
                {
                    log.LogInformation($"{model.InstanceId} is in status {existingInstance.RuntimeStatus}");
                    return;
                }
            }
            await starter.StartNewAsync(SearchSyncName, model.InstanceId, model);
        }

        [FunctionName(SearchSyncName)]
        public static async Task SearchSync(
            [OrchestrationTrigger] DurableOrchestrationContextBase context,
            ILogger log)
        {
            var input = context.GetInput<SearchSyncInput>();

            var query = await context.CallActivityAsync<SyncAzureQuery>(GetSyncStatusFunctionName, input.BlobName);

            //if (query.Version == 0 && query.Page == 0)
            //{

            //    await context.CallActivityAsync(CreateIndexFunctionName, input);
            //}
            input.SyncAzureQuery = query;

            bool needContinue;
            long nextVersion = 0;
            do
            {
                log.LogInformation($"start syncing {input.SyncType:G} with version {input.SyncAzureQuery.Version} page {input.SyncAzureQuery.Page}");
                var result = await context.CallActivityAsync<SyncResponse>(DoSyncFunctionName, input);
                needContinue = result.NeedContinue;
                nextVersion = Math.Max(nextVersion, result.Version);
                input.SyncAzureQuery.Page++;
                await context.CallActivityAsync(SetSyncStatusFunctionName, input);
            } while (needContinue);

            if (nextVersion == input.SyncAzureQuery.Version)
            {
                nextVersion++;
            }
            input.SyncAzureQuery = new SyncAzureQuery(Math.Max(nextVersion, input.SyncAzureQuery.Version), 0);
            await context.CallActivityAsync(SetSyncStatusFunctionName, input);
            log.LogInformation($"finish syncing {input.SyncType:G} with version {input.SyncAzureQuery.Version} page {input.SyncAzureQuery.Page}");


        }

        //[FunctionName(CreateIndexFunctionName)]
        //public static async Task CreateSearchIndex(
        //    [ActivityTrigger] SearchSyncInput input,
        //    [Inject] ILifetimeScope lifetimeScope,
        //    CancellationToken token)
        //{

        //    var syncObject = lifetimeScope.ResolveKeyed<IDbToSearchSync>(input.SyncType);
        //    await syncObject.CreateIndexAsync(token);
        //}

        [FunctionName(DoSyncFunctionName)]
        public static async Task<SyncResponse> DoSearchSync(
            [ActivityTrigger] SearchSyncInput input,
            [Inject] ILifetimeScope lifetimeScope,
            IBinder binder,
            ILogger log,
            CancellationToken token)
        {
            log.LogInformation($"Going to sync {input}");
            using (var child = lifetimeScope.BeginLifetimeScope())
            {
                var syncObject = child.ResolveKeyed<IDbToSearchSync>(input.SyncType);
                return await syncObject.DoSyncAsync(input.SyncAzureQuery, binder, token);
            }


        }

        [FunctionName(GetSyncStatusFunctionName)]
        public static async Task<SyncAzureQuery> GetSyncProgress(
            [ActivityTrigger] string blobName, IBinder binder, CancellationToken token)
        {
            var dynamicBlobAttribute =
                new BlobAttribute($"spitball/AzureSearch/{blobName}-version.txt");

            var blob = await binder.BindAsync<CloudBlockBlob>(dynamicBlobAttribute, token);
            if (!await blob.ExistsAsync()) return SyncAzureQuery.Empty();
            var text = await blob.DownloadTextAsync();
            return SyncAzureQuery.ConvertFromString(text);
        }


        [FunctionName(SetSyncStatusFunctionName)]
        public static async Task SetSyncProgress(
            [ActivityTrigger] SearchSyncInput searchSyncInput, IBinder binder, CancellationToken token)
        {
            var dynamicBlobAttribute =
                new BlobAttribute($"spitball/AzureSearch/{searchSyncInput.BlobName}-version.txt");

            var blob = await binder.BindAsync<CloudBlockBlob>(dynamicBlobAttribute, token);
            await blob.UploadTextAsync(searchSyncInput.SyncAzureQuery.ToString());
        }


    }
}