using System;
using System.Collections.Generic;
using System.IO;
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

namespace Cloudents.Functions
{
    [DependencyInjectionConfig(typeof(DiConfig))]
    public static class UniversityFunction
    {

        [FunctionName("UniversityTimer")]
        public static async Task RunAsync([TimerTrigger("0 */30 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Blob("spitball/AzureSearch/university-version.txt", FileAccess.Read)]  string blobRead,
            [Blob("spitball/AzureSearch/university-version.txt", FileAccess.Write)] TextWriter blobWrite,
            [Inject] IReadRepositoryAsync<(IEnumerable<UniversitySearchWriteDto> update, IEnumerable<UniversitySearchDeleteDto> delete, long version), long> repository,
            [Queue(QueueName.UrlRedirectName, Connection = "TempConnection")] IAsyncCollector<string> queue,
            TraceWriter log,
            CancellationToken token)
        {
            var version = long.Parse(blobRead);
            var data = await repository.GetAsync(version, token).ConfigureAwait(false);
            var tasks = new List<Task>();
            foreach (var dto in data.update)
            {
                tasks.Add(queue.AddAsStringAsync(dto, token));
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
            var obj = JsonConvertInheritance.DeserializeObject<UniversitySearchDeleteDto>(content);
            if (obj is UniversitySearchWriteDto write)
            {
                var university = new University
                {
                    Name = write.Name,
                    Image = write.Image,
                    Extra = write.Extra,
                    GeographyPoint = GeographyPoint.Create(write.Latitude, write.Longitude),
                    Id = write.Id.ToString(),
                    Prefix = write.Name
                };
                await searchServiceWrite.UpdateDataAsync(new[] { university }, token).ConfigureAwait(false);
            }
            else
            {
                await searchServiceWrite.DeleteDataAsync(new[] { obj.Id.ToString() }, token).ConfigureAwait(false);
            }
        }

        //[FunctionName("SynonymWatch")]
        //public static async Task SynonymWatchAsync([BlobTrigger("spitball/AzureSearch/{name}")]
        //    string content, string name, TraceWriter log,
        //    [Inject] ISynonymWrite synonymWrite,
        //    CancellationToken token)
        //{
        //    var fileName = Path.GetFileNameWithoutExtension(name);
        //    if (string.Equals(fileName, UniversitySearchWrite.SynonymName, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        await synonymWrite.CreateOrUpdateAsync(fileName, content, token).ConfigureAwait(false);
        //        log.Info(content);
        //    }
        //}
    }
}
