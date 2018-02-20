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
using Cloudents.Core.Storage;
using Cloudents.Infrastructure.Write;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Spatial;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Cloudents.Functions
{
    [DependencyInjectionConfig(typeof(DiConfig))]
    public static class UniversityFunction
    {

        [FunctionName("UniversityTimer")]
        public static async Task RunAsync([TimerTrigger("0 */30 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Blob("spitball/AzureSearch/university-version.txt", FileAccess.Read)]  string blobRead,
            [Blob("spitball/AzureSearch/university-version.txt", FileAccess.Write)] TextWriter blobWrite,
            [Inject] IReadRepositoryAsync<(List<UniversitySearchWriteDto> update, IEnumerable<UniversitySearchDeleteDto> delete, long version), long> repository,
            [Queue(QueueName.UrlRedirectName, Connection = "TempConnection")] IAsyncCollector<string> queue,
            //[Queue(QueueName.UrlRedirectName, Connection = "TempConnection")] IAsyncCollector<DeleteDto> queueDelete,
            TraceWriter log,
            CancellationToken token)
        {
            // blob.ReadFromStreamAsync()
            var version = long.Parse(blobRead);
            // int.Parse(await blob.DownloadTextAsync(token));
            var data = await repository.GetAsync(version, token).ConfigureAwait(false);
            var tasks = new List<Task>();
            foreach (var dto in data.update)
            {
                if (dto.IsDeleted)
                {
                    var deleted = new UniversitySearchDeleteDto(dto.Id);
                    tasks.Add(queue.AddAsStringAsync(deleted, token));
                }
                else
                {
                    tasks.Add(queue.AddAsStringAsync(dto, token));
                }
            }

            foreach (var d in data.delete)
            {
                tasks.Add(queue.AddAsStringAsync(d, token));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            if (data.version != version)
            {
                await blobWrite.WriteAsync(data.version.ToString()).ConfigureAwait(false);
            }

            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        [FunctionName("UniversityUpload")]
        public static async Task ProcessQueueAsync(
            [QueueTrigger(QueueName.UrlRedirectName, Connection = "TempConnection")] string content,
            [Inject] ISearchServiceWrite<University> searchServiceWrite,
            CancellationToken token
            )
        {
            var obj = JsonConvert.DeserializeObject<UniversitySearchDeleteDto>(content, AsyncCollectorExtensions.Settings);
            if (obj is UniversitySearchWriteDto write)
            {
                var university = new University
                {
                    Name = write.Name,
                    Image = write.Image,
                    Extra = null,
                    GeographyPoint = GeographyPoint.Create(write.Latitude, write.Longitude),
                    Id = write.Id.ToString()
                };
                await searchServiceWrite.UpdateDataAsync(new[] { university }, token);
            }
            else
            {
                await searchServiceWrite.DeleteDataAsync(new[] { obj.Id.ToString() }, token);
            }
            //searchServiceWrite.UpdateDataAsync()
        }

        [FunctionName("SynonymWatch")]
        public static async Task SynonymWatchAsync([BlobTrigger("spitball/AzureSearch/{name}")]
            string content, string name, TraceWriter log,
            [Inject] ISynonymWrite synonymWrite,
            CancellationToken token)
        {
            var fileName = Path.GetFileNameWithoutExtension(name);
            if (string.Equals(fileName, UniversitySearchWrite.SynonymName, StringComparison.InvariantCultureIgnoreCase))
            {
                await synonymWrite.CreateOrUpdateAsync(fileName, content, token).ConfigureAwait(false);
                log.Info(content);
            }
        }
    }
}
