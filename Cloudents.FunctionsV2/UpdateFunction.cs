using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
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
        //[FunctionName("UpdateReviewText")]
        //[SuppressMessage("ReSharper", "UnusedParameter.Global")]
        //[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used by azure function")]
        //public static async Task UpdateReviewTextAsync([TimerTrigger("0 0 1 * * *")] TimerInfo myTimer,
        //    [Inject] IStatelessSession statelessSession,
        //    ILogger log,
        //    CancellationToken token)
        //{
        //   await statelessSession.Query<ReadTutor>()
        //        .Where(w => w.Country == "IL" && w.SbCountry == null)
        //        .UpdateBuilder().Set(x => x.SbCountry, Country.Israel)
        //        .UpdateAsync(token);

        //   await statelessSession.Query<ReadTutor>()
        //       .Where(w => w.Country == "IN" && w.SbCountry == null)
        //       .UpdateBuilder().Set(x => x.SbCountry, Country.India)
        //       .UpdateAsync(token);

        //   await statelessSession.Query<ReadTutor>()
        //       .Where(w => w.SbCountry == null)
        //       .UpdateBuilder().Set(x => x.SbCountry, Country.UnitedStates)
        //       .UpdateAsync(token);
        //}

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
