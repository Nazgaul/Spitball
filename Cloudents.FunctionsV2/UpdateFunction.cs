using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Query;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class UpdateFunction
    {
        [FunctionName("UpdateReviewText")]
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used by azure function")]
        public static async Task UpdateReviewTextAsync([TimerTrigger("0 0 1 * * *")] TimerInfo myTimer,
            [Inject] IDapperRepository dapperRepository,
            ILogger log)
        {
            using var openConnection = dapperRepository.OpenConnection();
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

        [FunctionName("DeleteOldUserLocation")]
        public static async Task DeleteOldUserLocationAsync([TimerTrigger("0 0 2 * * *")] TimerInfo myTimer,
            [Inject] IStatelessSession statelessSession,
            ILogger log,
            CancellationToken token)
        {
            var i = await statelessSession.Query<UserLocation>()
                  .Where(w => w.TimeStamp.CreationTime < DateTime.UtcNow.AddYears(-1))
                  .DeleteAsync(token);

            log.LogInformation($"deleted amount: {i}");
        }


        [FunctionName("UpdateSbCountry")]
        public static async Task UpdateSbCountryAsync([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
            [Inject] IDapperRepository dapper,
            ILogger log,
            CancellationToken token)
        {
            var sql = @"update top (1000) sb.[User]
            set SbCountry = 1
            where Country = 'IL' and SbCountry is null";

            var sql2 = @"update top (1000) sb.[User]
set SbCountry = 2
where Country = 'IN' and SbCountry is null";

            var sql3 = @"update top (1000) sb.[User]
set SbCountry = 3
where SbCountry is null and country not in ('IL','IN') ";

            var queries = new[] { sql, sql2, sql3 };

            foreach (var query in queries)
            {
                using var con = dapper.OpenConnection();
                while (true)
                {
                    var i = await con.ExecuteAsync(query);
                    if (i == 0)
                    {
                        break;
                    }
                }
            }
        }
    }
}
