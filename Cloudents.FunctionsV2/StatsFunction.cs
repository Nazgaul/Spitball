using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class StatsFunction
    {
        [FunctionName("UpdateStatsFunction")]
        public static async Task Run([TimerTrigger("0 0 0 * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Inject] ICommandBus commandBus,
            ILogger log,
            CancellationToken token)
        {
            log.LogInformation("UpdateStatsFunction invoke");
            var command = new UpdateHomeStatsCommand();
            await commandBus.DispatchAsync(command, token);
            log.LogInformation($"UpdateStatsFunction function executed at: {DateTime.Now}");
        }
    }
}
