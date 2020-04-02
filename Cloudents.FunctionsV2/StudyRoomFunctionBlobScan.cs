//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.Interfaces;
//using Cloudents.Core.Storage;
//using Cloudents.FunctionsV2.FileProcessor;
//using Cloudents.Query;
//using Cloudents.Query.Email;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Extensions.Logging;
//using Microsoft.WindowsAzure.Storage.Blob;
//using Newtonsoft.Json;
//using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
//using ILogger = Microsoft.Extensions.Logging.ILogger;

//namespace Cloudents.FunctionsV2
//{
//    public static class StudyRoomFunctionBlobScan
//    {
//        //Guid doesn't have s in ToString method and container cant be opened with _ so we replace that
//        public const char Separator = 's';
//        public const string MetaEncodingKey = "encodingState";

//        [FunctionName("StudyRoomBlobFunction")]
//        public static async Task RunAsync(
//            [BlobTrigger("spitball-files/study-room/{studyRoomId}/{studyRoomId2}_{sessionId}.mp4")]CloudBlockBlob blob, string studyRoomId2, string sessionId,
//            [Inject] IVideoService videoService,
//            [Inject] IQueryBus queryBus,
//            [Queue(QueueName.BackgroundQueueName)] IAsyncCollector<string> queueCollector,
//            ILogger log,
//            CancellationToken token)
//        {
//            log.LogInformation($"Received blob {blob.Name}");
//            await blob.FetchAttributesAsync();

//            if (blob.Metadata.TryGetValue(MetaEncodingKey, out _))
//            {
//                log.LogInformation($"Sending email of blob {blob.Name}");
//                var query = new StudyRoomVideoEmailQuery($"{studyRoomId2}_{sessionId}");
//                var result = await queryBus.QueryAsync(query, token);
//                result.DownloadLink = blob.GetDownloadLink(TimeSpan.FromDays(365));

//                var json = JsonConvert.SerializeObject(result, new JsonSerializerSettings
//                {
//                    TypeNameHandling = TypeNameHandling.All
//                });
//                await queueCollector.AddAsync(json, token);
//                return;
//            }
//            log.LogInformation($"Processing video of blob {blob.Name}");

//            var link = blob.GetDownloadLink(TimeSpan.FromHours(1));
//            var v = $"{studyRoomId2}{Separator}{sessionId}";
//            await videoService.CreateStudyRoomSessionEncoding(v, link.AbsoluteUri, token);
//        }
//    }
//}
