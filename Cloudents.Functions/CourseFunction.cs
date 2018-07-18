using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Functions.Di;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.Functions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Azure function")]
    public static class CourseFunction
    {
       // [FunctionName("CourseTimer")]
        public static async Task RunAsync([TimerTrigger("0 */30 * * * *")]TimerInfo myTimer,
            [Blob("spitball/AzureSearch/course-version.txt", FileAccess.ReadWrite)]
            CloudBlockBlob blob,
            [Inject] IReadRepositoryAsync<(IEnumerable<CourseSearchWriteDto> update, IEnumerable<SearchWriteBaseDto> delete, long version), SyncAzureQuery> repository,
            [Inject] ISearchServiceWrite<Course> searchServiceWrite,
            TraceWriter log,
            CancellationToken token)
        {
            if (myTimer.IsPastDue)
            {
                log.Info("pass due run.");
                return;
            }
            await SyncFunc.SyncAsync(blob, repository, searchServiceWrite, write => new Course
            {
                Name = write.Name,
                Code = write.Code,
                UniversityId = write.UniversityId,
                Id = write.Id.ToString(),
                Prefix = new[] { write.Name, write.Code }.Where(x => x != null).ToArray()
            },log, token).ConfigureAwait(false);
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
