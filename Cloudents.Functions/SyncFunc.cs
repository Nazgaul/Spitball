using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.Functions
{
    public static class SyncFunc
    {
        public static async Task SyncAsync<T,TU>(
            CloudBlockBlob blob,
            IReadRepositoryAsync<(IEnumerable<T> update, IEnumerable<SearchWriteBaseDto> delete, long version),
                SyncAzureQuery> repository,
            ISearchServiceWrite<TU> searchServiceWrite,
            Func<T,TU> createObj,
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

            if (!token.IsCancellationRequested)
            {
                var newVersion = new SyncAzureQuery(currentVersion, 0);
                await blob.UploadTextAsync(newVersion.ToString(), token).ConfigureAwait(false);
            }
        }
    }
}