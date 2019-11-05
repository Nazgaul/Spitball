//using System;
//using System.Threading.Tasks;
//using Cloudents.Core.Event;
//using Cloudents.Core.Interfaces;
//using Cloudents.Query;
//using Dapper;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Host;
//using Microsoft.Extensions.Logging;
//using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
//using ILogger = Microsoft.Extensions.Logging.ILogger;

//namespace Cloudents.FunctionsV2
//{
//    public static class UpdateFunction
//    {
//        [FunctionName("UpdateFunction")]
//        public static async Task Run([TimerTrigger("0 0 0 0 0 */1", RunOnStartup = true)]TimerInfo myTimer,
//            [Inject] IDapperRepository dapperRepository,
//            [Inject] IEventHandler<TutorAddReviewEvent> eventHandler,
//            ILogger log)
//        {
//            using (var openConnection = dapperRepository.OpenConnection())
//            {
//                var sql = @"update sb.document
//set description = null
//where description = ''";
//                await openConnection.ExecuteAsync(sql);


//                var sql2 =
//                    @"Select id from sb.readtutor t where rateCount != (select count(*) from sb.TutorReview where tutorid = t.id)";
//                var result = await openConnection.QueryAsync<long>(sql2);
//                foreach (var userId in result)
//                {
//                    var @event = new TutorAddReviewEvent(userId);
//                    await eventHandler.HandleAsync(@event, default);
//                }

//            }

//            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
//        }
//    }
//}
