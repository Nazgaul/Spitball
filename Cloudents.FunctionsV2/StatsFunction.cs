using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Cloudents.Query;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class StatsFunction
    {
        [FunctionName("UpdateStatsFunction")]
        public static async Task Run([TimerTrigger("0 0 0 * * *")]TimerInfo myTimer,
            [Inject] DapperRepository repository,
            ILogger log,
            CancellationToken token)
        {
            log.LogInformation("UpdateStatsFunction invoke");
            using (var conn = repository.OpenConnection())
            {
                await conn.ExecuteAsync(@"update sb.[HomeStats]
                            set[users] = (select count(1) from sb.[User] where PhoneNumberConfirmed = 1 and EmailConfirmed = 1 and Fictive = 0)
	                        ,[answers] = (select count(1) from sb.Answer where state = 'ok')
	                        ,[SBLs] = (select sum(price) from sb.[Transaction] where [Type] = 'Earned')");
            }
            log.LogInformation($"UpdateStatsFunction function executed at: {DateTime.Now}");
        }
    }
}
