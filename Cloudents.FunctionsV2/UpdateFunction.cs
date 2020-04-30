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
            int count;
            do
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                var v = await statelessSession.Query<UserLocation>()
                    .Where(w => w.TimeStamp.CreationTime < DateTime.UtcNow.AddYears(-1))
                    .OrderBy(o => o.Id)
                    .Take(100)
                    .Select(s => s.Id).ToListAsync(cancellationToken: token);
                count = v.Count;
                if (count > 0)
                {
                    var x =  await statelessSession.Query<UserLocation>()
                        .Where(w => v.Contains(w.Id))
                        .DeleteAsync(default);
                    log.LogInformation($"deleted amount: {x}");
                }
            } while (count > 0);
        
        }
    }
}
