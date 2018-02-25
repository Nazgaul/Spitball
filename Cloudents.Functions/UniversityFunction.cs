using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Spatial;

namespace Cloudents.Functions
{
    [DependencyInjectionConfig(typeof(DiConfig))]
    public static class UniversityFunction
    {
        private const string QueueName = "university-sync";

        //[FunctionName("UniversityTimer")]
        [UsedImplicitly]
        public static async Task RunAsync([TimerTrigger("0 */30 * * * *")]TimerInfo myTimer,
            [Blob("spitball/AzureSearch/university-version.txt", FileAccess.Read),CanBeNull]  string blobRead,
            [Blob("spitball/AzureSearch/university-version.txt", FileAccess.Write)] TextWriter blobWrite,
            [Inject] IReadRepositoryAsync<(IEnumerable<UniversitySearchWriteDto> update, IEnumerable<SearchWriteBaseDto> delete, long version), long> repository,
            [Inject] ISearchServiceWrite<University> searchServiceWrite,
            [Queue(QueueName)] IAsyncCollector<string> queue,
            TraceWriter log,
            CancellationToken token)
        {
            var version = long.Parse(blobRead ?? "0");
            var t1 = Task.CompletedTask;
            if (version == 0)
            {
                t1 = searchServiceWrite.CreateOrUpdateAsync(token);
            }
            var dataTask = repository.GetAsync(version, token);
            await Task.WhenAll(t1, dataTask).ConfigureAwait(false);
            var data = dataTask.Result;
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
        [UsedImplicitly]
        public static async Task ProcessQueueAsync(
            [QueueTrigger(QueueName)] string content,
            [Inject] ISearchServiceWrite<University> searchServiceWrite,
            CancellationToken token
            )
        {
            var obj = JsonConvertInheritance.DeserializeObject<SearchWriteBaseDto>(content);
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
    }
}
