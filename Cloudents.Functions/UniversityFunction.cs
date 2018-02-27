using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using JetBrains.Annotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Spatial;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.Functions
{
    [DependencyInjectionConfig(typeof(DiConfig))]
    public static class UniversityFunction
    {
        [FunctionName("UniversityTimer")]
        [UsedImplicitly]
        public static async Task RunAsync([TimerTrigger("0 */30 * * * *")]TimerInfo myTimer,
            [Blob("spitball/AzureSearch/university-version.txt",FileAccess.ReadWrite)]
            CloudBlockBlob blob,
            [Inject] IReadRepositoryAsync<(IEnumerable<UniversitySearchWriteDto> update, IEnumerable<SearchWriteBaseDto> delete, long version), SyncAzureQuery> repository,
            [Inject] ISearchServiceWrite<University> searchServiceWrite,
            TraceWriter log,
            CancellationToken token)
        {
            var text = await blob.DownloadTextAsync(token).ConfigureAwait(false);
            var query = SyncAzureQuery.ConvertFromString(text);
            if (query.Version == 0)
            {
                await searchServiceWrite.CreateOrUpdateAsync(token).ConfigureAwait(false);
            }

            var currentVersion = query.Version;
            while (!token.IsCancellationRequested)
            {
                var (update, delete, version) = await repository.GetAsync(query, token).ConfigureAwait(false);
                var universityUpdates = update.Select(s => new University
                {
                    Name = s.Name,
                    Image = s.Image,
                    Extra = s.Extra,
                    GeographyPoint = GeographyPoint.Create(s.Latitude, s.Longitude),
                    Id = s.Id.ToString(),
                    Prefix = s.Name
                }).ToList();
                var deleteUniversity = delete.Select(s => s.Id.ToString()).ToList();
                await  searchServiceWrite.UpdateDataAsync(universityUpdates, deleteUniversity, token).ConfigureAwait(false);
                query.Page++;
                currentVersion = Math.Max(currentVersion, version);
                await blob.UploadTextAsync(query.ToString(), token).ConfigureAwait(false);
                if (universityUpdates.Count == 0 && deleteUniversity.Count == 0)
                {
                    break;
                }
            }

            var newVersion = new SyncAzureQuery(currentVersion, 0);
            await blob.UploadTextAsync(newVersion.ToString(), token).ConfigureAwait(false);

            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        //[FunctionName("UniversityUpload")]
        //[UsedImplicitly]
        //public static async Task ProcessQueueAsync(
        //    [QueueTrigger(QueueName)] string content,
        //    [Inject] ISearchServiceWrite<University> searchServiceWrite,
        //    CancellationToken token
        //    )
        //{
        //    var obj = JsonConvertInheritance.DeserializeObject<SearchWriteBaseDto>(content);
        //    if (obj is UniversitySearchWriteDto write)
        //    {
        //        var university = new University
        //        {
        //            Name = write.Name,
        //            Image = write.Image,
        //            Extra = write.Extra,
        //            GeographyPoint = GeographyPoint.Create(write.Latitude, write.Longitude),
        //            Id = write.Id.ToString(),
        //            Prefix = write.Name
        //        };
        //        await searchServiceWrite.UpdateDataAsync(new[] { university }, token).ConfigureAwait(false);
        //    }
        //    else
        //    {
        //        await searchServiceWrite.DeleteDataAsync(new[] { obj.Id.ToString() }, token).ConfigureAwait(false);
        //    }
        //}
    }
}
