using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using SendGrid.Helpers.Mail;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class UpdateFunction
    {

        [FunctionName("DeleteTutorsForEidan")]
        public static async Task DeleteTutorCheckForEidan(
            [TimerTrigger("0 0 * * * *")] TimerInfo timer,
            [Inject] IStatelessSession statelessSession,
            [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>")] IAsyncCollector<SendGridMessage> emailProvider,
            CancellationToken token
            )
        {
            const string sql = @"select ID, email, country, created from sb.[user] where id in
(Select ID from sb.TutorHistory where created> getdate()-2
except
Select ID from sb.Tutor)
and id not in (525015,525046,525055)";

            var result = await statelessSession.CreateSQLQuery(sql).ListAsync();
            if (result.Count > 0)
            {
                var message = new SendGridMessage()
                {

                    Subject = "A tutor/s was deleted",
                    PlainTextContent = $"A tutor/s was deleted {result.Count}"
                };
                message.AddTo("eidan@cloudents.com");
                await emailProvider.AddAsync(message, token);
            }
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
