using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Cloudents.Query;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class UpdateFunction
    {
        [FunctionName("UpdateReviewTest")]
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used by azure function")]
        public static async Task UpdateReviewTestAsync([TimerTrigger("0 0 1 * * *")] TimerInfo myTimer,
            [Inject] IDapperRepository dapperRepository,
            ILogger log)
        {
            using (var openConnection = dapperRepository.OpenConnection())
            {
                const string sql = @" update tr
                    set Review = x.[text]
                    from sb.TutorReview tr
                join
                    (
                    Select  tri.Id, case when floor(Rate) = 1 then N'Not recommended'
                     when floor(Rate) = 2 then N'Ok'
                     when floor(Rate) = 3 then N'Good'
                     when floor(Rate) = 4 then N'Very Good'
                     when floor(Rate) = 5 then N'Excellent' end as Text
                from sb.TutorReview tri
                join sb.tutor t on tri.TutorId = t.id
                    join sb.[user] u on u.id = t.id
                    where tri.Review is null
                     and u.Country != 'IL'
                    ) x
                    on x.Id = tr.Id";

                var result = await openConnection.ExecuteAsync(sql);

                log.LogInformation($"update non IL amount: {result}");

                const string ilSQl = @"update tr
set Review = x.[text]
from sb.TutorReview tr
join
(
Select tri.Id, case when floor(tri.Rate) = 1 then N'לא מומלץ'
 when floor(tri.Rate) = 2 then N'בסדר'
 when floor(tri.Rate) = 3 then N'טוב'
 when floor(tri.Rate) = 4 then N'טוב מאוד'
 when floor(tri.Rate) = 5 then N'מצויין' end  as [text]
from sb.TutorReview tri
join sb.tutor t on tri.TutorId = t.id
join sb.[user] u on u.id = t.id
where tri.Review is null
 and u.Country = 'IL'
) x
on x.Id = tr.Id";

                result = await openConnection.ExecuteAsync(ilSQl);
                log.LogInformation($"update IL amount: {result}");
            }
        }

        //[FunctionName("UpdateFunction")]
        //[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used by azure function")]
        //public static async Task Run([TimerTrigger("0 0 0 1 */1 *", RunOnStartup = true)]TimerInfo myTimer,
        //    [Inject] IDapperRepository dapperRepository,
        //    [Blob("spitball/AzureSearch/tutor-version.txt")] CloudBlockBlob blob,
        //    ILogger log)
        //{
        //    //await blob.DeleteIfExistsAsync();
        //    //            using (var openConnection = dapperRepository.OpenConnection())
        //    //            {
        //    ////                var sql = @"update sb.tutor
        //    ////set SubsidizedPrice = null
        //    ////where SubsidizedPrice = price ";
        //    ////                await openConnection.ExecuteAsync(sql);

        //    ////                var sqlX = @"update sb.tutor
        //    ////set SubsidizedPrice = 0,price = 100
        //    ////where id in (
        //    ////Select t.id from sb.[user] u join sb.tutor t on u.id = t.id and u.country = 'IN')";

        //    ////                await openConnection.ExecuteAsync(sqlX);


        //    ////                var sqlY = @"update sb.tutor
        //    ////set SubsidizedPrice = null
        //    ////where id in (
        //    ////Select t.id from sb.[user] u join sb.tutor t on u.id = t.id and u.country != 'IN')";

        //    ////                await openConnection.ExecuteAsync(sqlY);

        //    //                //var sql2 =
        //    //                //    @"Select id from sb.tutor t where t.State = 'Ok'";
        //    //                //var result = await openConnection.QueryAsync<long>(sql2);
        //    //                //foreach (var userId in result)
        //    //                //{
        //    //                //    var @event = new TutorAddReviewEvent(userId);
        //    //                //    await eventHandler.HandleAsync(@event, default);
        //    //                //}

        //    //            }

        //    log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        //}
    }
}
