﻿using Autofac;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Functions.Di;
using Cloudents.Functions.Sync;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Functions
{
    public static class SyncFunc
    {
        public static async Task SyncAsync<T, TU>(
            CloudBlockBlob blob,
            IReadRepositoryAsync<(IEnumerable<T> update, IEnumerable<SearchWriteBaseDto> delete, long version),
                SyncAzureQuery> repository,
            ISearchServiceWrite<TU> searchServiceWrite,
            Func<T, TU> createObj,
            TraceWriter log,
            CancellationToken token) where TU : class, ISearchObject, new()
        {
            var query = SyncAzureQuery.Empty();
            if (await blob.ExistsAsync(token).ConfigureAwait(false))
            {
                var text = await blob.DownloadTextAsync(token).ConfigureAwait(false);
                query = SyncAzureQuery.ConvertFromString(text);
            }
            log.Info($"process {query}");

            //if (query.Version == 0 && query.Page == 0)
            //{
            //    await searchServiceWrite.CreateOrUpdateAsync(token).ConfigureAwait(false);
            //}

            var currentVersion = query.Version;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var (update, delete, version) = await repository.GetAsync(query, token).ConfigureAwait(false);

                    var updateList = update.Select(createObj).ToList();
                    var deleteCourses = delete.Select(s => s.Id.ToString()).ToList();
                    await searchServiceWrite.UpdateDataAsync(updateList, deleteCourses, token).ConfigureAwait(false);
                    query.Page++;
                    currentVersion = Math.Max(currentVersion, version);
                    await blob.UploadTextAsync(query.ToString(), token).ConfigureAwait(false);
                    if (updateList.Count == 0 && deleteCourses.Count == 0)
                    {
                        break;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            if (!token.IsCancellationRequested)
            {
                var newVersion = new SyncAzureQuery(currentVersion, 0);
                await blob.UploadTextAsync(newVersion.ToString(), token).ConfigureAwait(false);
            }
        }

        [FunctionName("SearchSync")]
        public static async Task SearchSync(
            [OrchestrationTrigger] DurableOrchestrationContextBase context,
            [Inject] ILifetimeScope lifetimeScope,
            CancellationToken token)
        {
            var input = context.GetInput<SearchSyncInput>();

            var query = await context.CallActivityAsync<SyncAzureQuery>("GetSyncProgress", input.BlobName);

            var syncObject = lifetimeScope.ResolveKeyed<IDbToSearchSync>(input.SyncType);

            if (query.Version == 0 && query.Page == 0)
            {
                await syncObject.CreateIndexAsync(token).ConfigureAwait(false);
            }

            do
            {
                
            } while (b);

        }

        [FunctionName("DoSearchSync")]
        public static async Task<SyncAzureQuery> DoSearchSync(
            [ActivityTrigger] SyncAzureQuery syncAzureQuery,
            [Inject] ILifetimeScope lifetimeScope,
            CancellationToken token)
        {

            var syncObject = lifetimeScope.ResolveKeyed<IDbToSearchSync>(input.SyncType);
            //var dynamicBlobAttribute =
            //    new BlobAttribute($"spitball/AzureSearch/{blobName}-version.txt");

            //var blob = await binder.BindAsync<CloudBlockBlob>(dynamicBlobAttribute, token).ConfigureAwait(false);
            //if (await blob.ExistsAsync(token).ConfigureAwait(false))
            //{
            //    var text = await blob.DownloadTextAsync(token).ConfigureAwait(false);
            //    return SyncAzureQuery.ConvertFromString(text);
            //}
            //return SyncAzureQuery.Empty();
        }

        [FunctionName("GetSyncProgress")]
        public static async Task<SyncAzureQuery> GetSyncProgress(
            [ActivityTrigger] string blobName, IBinder binder, CancellationToken token)
        {
            var dynamicBlobAttribute =
                new BlobAttribute($"spitball/AzureSearch/{blobName}-version.txt");

            var blob = await binder.BindAsync<CloudBlockBlob>(dynamicBlobAttribute, token).ConfigureAwait(false);
            if (await blob.ExistsAsync(token).ConfigureAwait(false))
            {
                var text = await blob.DownloadTextAsync(token).ConfigureAwait(false);
                return SyncAzureQuery.ConvertFromString(text);
            }
            return SyncAzureQuery.Empty();
        }

        public static async Task SyncAsync2<T, TU>(
            CloudBlockBlob blob,
            //IQueryBus bus,
            ISearchServiceWrite<TU> searchServiceWrite,
            Func<T, TU> createObj,
            Func<SyncAzureQuery, Task<(IEnumerable<T> update, IEnumerable<long> delete, long version)>> getDataFunc,
            TraceWriter log,
            CancellationToken token) where TU : class, ISearchObject, new()
        {
            var query = SyncAzureQuery.Empty();
            if (await blob.ExistsAsync(token).ConfigureAwait(false))
            {
                var text = await blob.DownloadTextAsync(token).ConfigureAwait(false);
                query = SyncAzureQuery.ConvertFromString(text);
            }
            log.Info($"process {query}");

            if (query.Version == 0 && query.Page == 0)
            {
                await searchServiceWrite.CreateOrUpdateAsync(token).ConfigureAwait(false);
            }

            var currentVersion = query.Version;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    //var (update, delete, version) = await bus.QueryAsync<(IEnumerable<T> update, IEnumerable<long> delete, long version)>(query, token);
                    var (update, delete, version) = await getDataFunc(query);
                    var updateList = update.Select(createObj).ToList();
                    var deleteCourses = delete.Select(s => s.ToString()).ToList();
                    await searchServiceWrite.UpdateDataAsync(updateList, deleteCourses, token).ConfigureAwait(false);
                    query.Page++;
                    currentVersion = Math.Max(currentVersion, version);
                    await blob.UploadTextAsync(query.ToString(), token).ConfigureAwait(false);
                    if (updateList.Count == 0 && deleteCourses.Count == 0)
                    {
                        break;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            if (!token.IsCancellationRequested)
            {
                var newVersion = new SyncAzureQuery(currentVersion, 0);
                await blob.UploadTextAsync(newVersion.ToString(), token).ConfigureAwait(false);
            }
        }
    }
}