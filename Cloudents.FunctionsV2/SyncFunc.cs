﻿using Autofac;
using Cloudents.Core.Query.Sync;
using Cloudents.FunctionsV2.Sync;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class SyncFunc
    {

        internal static async Task StartSearchSync(DurableOrchestrationClient starter,
            ILogger log, SyncType syncType)
        {
            var model = new SearchSyncInput(syncType);
            var existingInstance = await starter.GetStatusAsync(model.InstanceId);
            var startNewInstanceEnum = new[]
            {
                OrchestrationRuntimeStatus.Canceled,
                OrchestrationRuntimeStatus.Completed,
                OrchestrationRuntimeStatus.Failed,
                OrchestrationRuntimeStatus.Terminated
            };

            if (existingInstance == null)
            {
                await starter.StartNewAsync("SearchSync", model.InstanceId, model);
                return;

            }
            if (startNewInstanceEnum.Contains(existingInstance.RuntimeStatus))
            {
                if (existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Failed)
                {
                    await starter.TerminateAsync(model.InstanceId, "the status failed");
                }

                log.LogInformation($"started {model.InstanceId}");

                await starter.StartNewAsync("SearchSync", model.InstanceId, model);
            }
            else
            {
                log.LogInformation($"{model.InstanceId} is in status {existingInstance.RuntimeStatus}");

            }
        }

        [FunctionName("SearchSync")]
        public static async Task SearchSync(
            [OrchestrationTrigger] DurableOrchestrationContextBase context)
        {
            var input = context.GetInput<SearchSyncInput>();

            var query = await context.CallActivityAsync<SyncAzureQuery>("GetSyncProgress", input.BlobName);

            if (query.Version == 0 && query.Page == 0)
            {
                await context.CallActivityAsync("CreateSearchIndex", input);
            }
            input.SyncAzureQuery = query;

            bool needContinue;
            long nextVersion = 0;
            do
            {

                var result = await context.CallActivityAsync<SyncResponse>("DoSearchSync", input);
                needContinue = result.NeedContinue;
                nextVersion = Math.Max(nextVersion, result.Version);
                input.SyncAzureQuery.Page++;
            } while (needContinue);

            if (nextVersion == input.SyncAzureQuery.Version)
            {
                nextVersion++;
            }
            input.SyncAzureQuery = new SyncAzureQuery(nextVersion, 0);
            await context.CallActivityAsync("SetSyncProgress", input);

        }

        [FunctionName("CreateSearchIndex")]
        public static async Task CreateSearchIndex(
            [ActivityTrigger] SearchSyncInput input,
            [Inject] ILifetimeScope lifetimeScope,
            CancellationToken token)
        {

            var syncObject = lifetimeScope.ResolveKeyed<IDbToSearchSync>(input.SyncType);
            await syncObject.CreateIndexAsync(token);
        }

        [FunctionName("DoSearchSync")]
        public static async Task<SyncResponse> DoSearchSync(
            [ActivityTrigger] SearchSyncInput input,
            [Inject] ILifetimeScope lifetimeScope,
            CancellationToken token)
        {

            var syncObject = lifetimeScope.ResolveKeyed<IDbToSearchSync>(input.SyncType);
            return await syncObject.DoSyncAsync(input.SyncAzureQuery, token);
        }

        [FunctionName("GetSyncProgress")]
        public static async Task<SyncAzureQuery> GetSyncProgress(
            [ActivityTrigger] string blobName, IBinder binder, CancellationToken token)
        {
            var dynamicBlobAttribute =
                new BlobAttribute($"spitball/AzureSearch/{blobName}-version.txt");

            var blob = await binder.BindAsync<CloudBlockBlob>(dynamicBlobAttribute, token).ConfigureAwait(false);
            if (!await blob.ExistsAsync().ConfigureAwait(false)) return SyncAzureQuery.Empty();
            var text = await blob.DownloadTextAsync().ConfigureAwait(false);
            return SyncAzureQuery.ConvertFromString(text);
        }


        [FunctionName("SetSyncProgress")]
        public static async Task SetSyncProgress(
            [ActivityTrigger] SearchSyncInput searchSyncInput, IBinder binder, CancellationToken token)
        {
            var dynamicBlobAttribute =
                new BlobAttribute($"spitball/AzureSearch/{searchSyncInput.BlobName}-version.txt");

            var blob = await binder.BindAsync<CloudBlockBlob>(dynamicBlobAttribute, token).ConfigureAwait(false);
            await blob.UploadTextAsync(searchSyncInput.SyncAzureQuery.ToString()).ConfigureAwait(false);
        }
    }
}