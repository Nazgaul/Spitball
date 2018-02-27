using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.Functions
{
    [DependencyInjectionConfig(typeof(DiConfig))]
    public static class CourseFunction
    {
        private const string QueueName = "course-sync";

        [FunctionName("CourseTimer")]
        [UsedImplicitly]
        public static async Task RunAsync([TimerTrigger("0 */30 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Blob("spitball/AzureSearch/course-version.txt", FileAccess.ReadWrite)]
            CloudBlockBlob blob,
            [Inject] IReadRepositoryAsync<(IEnumerable<CourseSearchWriteDto> update, IEnumerable<SearchWriteBaseDto> delete, long version), SyncAzureQuery> repository,
            [Inject] ISearchServiceWrite<Course> searchServiceWrite,
            //[Inject] IBinarySerializer serializer,
           // [Queue(QueueName)] byte[] queue,
            TraceWriter log,
            CancellationToken token)
        {
            await SyncFunc.SyncAsync(blob, repository, searchServiceWrite, write => new Course
            {
                Name = write.Name,
                Code = write.Code,
                UniversityId = write.UniversityId,
                Id = write.Id.ToString(),
                Prefix = write.Name
            }, token).ConfigureAwait(false);
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

        }

        //[FunctionName("CourseUpload")]
        //[UsedImplicitly]
        //public static async Task ProcessQueueAsync(
        //    [QueueTrigger(QueueName)] string content,
        //    [Inject] ISearchServiceWrite<Course> searchServiceWrite,
        //    CancellationToken token
        //    )
        //{
        //    var obj = JsonConvertInheritance.DeserializeObject<SearchWriteBaseDto>(content);
        //    if (obj is CourseSearchWriteDto write)
        //    {
        //        var course = new Course
        //        {
        //            Name = write.Name,
        //            Code = write.Code,
        //            UniversityId = write.UniversityId,
        //            Id = write.Id.ToString(),
        //            Prefix = write.Name
        //        };
        //        await searchServiceWrite.UpdateDataAsync(new[] { course }, token).ConfigureAwait(false);
        //    }
        //    else
        //    {
        //        await searchServiceWrite.DeleteDataAsync(new[] { obj.Id.ToString() }, token).ConfigureAwait(false);
        //    }
        //}
    }
}
