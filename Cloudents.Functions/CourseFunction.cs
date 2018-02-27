using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using JetBrains.Annotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Functions
{
    [DependencyInjectionConfig(typeof(DiConfig))]
    public static class CourseFunction
    {
        private const string QueueName = "course-sync";

        //[FunctionName("CourseTimer")]
        [UsedImplicitly]
        public static async Task RunAsync([TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Blob("spitball/AzureSearch/course-version.txt", FileAccess.Read), CanBeNull]
            string blobRead,
            [Blob("spitball/AzureSearch/course-version.txt", FileAccess.Write)]
            TextWriter blobWrite,
            [Inject] IReadRepositoryAsync<(IEnumerable<CourseSearchWriteDto> update, IEnumerable<SearchWriteBaseDto> delete, long version), SyncAzureQuery> repository,
            [Inject] ISearchServiceWrite<Course> searchServiceWrite,
            [Queue(QueueName)] IAsyncCollector<string> queue,
            TraceWriter log,
            CancellationToken token)
        {

            var query = SyncAzureQuery.ConvertFromString(blobRead);
            var t1 = Task.CompletedTask;
            if (query.Version == 0)
            {
                t1 = searchServiceWrite.CreateOrUpdateAsync(token);
            }
            var dataTask = repository.GetAsync(query, token);
            await Task.WhenAll(t1, dataTask).ConfigureAwait(false);
            var data = dataTask.Result;
            var tasks = data.update.Batch(20).Select(dto => queue.AddAsStringAsync(dto, token));
            tasks = tasks.Union(data.delete.Batch(20).Select(d => queue.AddAsStringAsync(d, token)));


            await Task.WhenAll(tasks).ConfigureAwait(false);
            if (data.version != query.Version)
            {
                await blobWrite.WriteAsync(data.version.ToString()).ConfigureAwait(false);
            }

            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        [FunctionName("CourseUpload")]
        [UsedImplicitly]
        public static async Task ProcessQueueAsync(
            [QueueTrigger(QueueName)] string content,
            [Inject] ISearchServiceWrite<Course> searchServiceWrite,
            CancellationToken token
            )
        {
            var obj = JsonConvertInheritance.DeserializeObject<SearchWriteBaseDto>(content);
            if (obj is CourseSearchWriteDto write)
            {
                var course = new Course
                {
                    Name = write.Name,
                    Code = write.Code,
                    UniversityId = write.UniversityId,
                    Id = write.Id.ToString(),
                    Prefix = write.Name
                };
                await searchServiceWrite.UpdateDataAsync(new[] { course }, token).ConfigureAwait(false);
            }
            else
            {
                await searchServiceWrite.DeleteDataAsync(new[] { obj.Id.ToString() }, token).ConfigureAwait(false);
            }
        }
    }
}
