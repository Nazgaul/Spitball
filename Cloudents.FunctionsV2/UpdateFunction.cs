using System;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class UpdateFunction
    {
        [FunctionName("UpdateFunction")]
        public static async Task Run([TimerTrigger("0 0 0 1 */1 *", RunOnStartup = true)]TimerInfo myTimer,
            [Inject] IDapperRepository dapperRepository,
            [Inject] IEventHandler<TutorAddReviewEvent> eventHandler,
            [Blob("spitball/AzureSearch/tutor-version.txt")] CloudBlockBlob blob,
            ILogger log)
        {
            await blob.DeleteIfExistsAsync();
            using (var openConnection = dapperRepository.OpenConnection())
            {
                var sql = @"update sb.tutor
set SubsidizedPrice = null
where SubsidizedPrice = price ";
                await openConnection.ExecuteAsync(sql);

                var sqlX = @"update sb.tutor
set SubsidizedPrice = 0,price = 100
where id in (
Select t.id from sb.[user] u join sb.tutor t on u.id = t.id and u.country = 'IN')";

                await openConnection.ExecuteAsync(sqlX);


                var sqlY = @"update sb.tutor
set SubsidizedPrice = null
where id in (
Select t.id from sb.[user] u join sb.tutor t on u.id = t.id and u.country != 'IN')";

                await openConnection.ExecuteAsync(sqlY);

                var sql2 =
                    @"Select id from sb.tutor t where t.State = 'Ok'";
                var result = await openConnection.QueryAsync<long>(sql2);
                foreach (var userId in result)
                {
                    var @event = new TutorAddReviewEvent(userId);
                    await eventHandler.HandleAsync(@event, default);
                }

            }

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
