using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class Test
    {
        //[FunctionName("Test")]
        public static async Task Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Inject] ICommandBus commandBus,
            ILogger log,
            CancellationToken token)
        {
            var command = new UpdateQuestionTimeCommand();
            await commandBus.DispatchAsync(command, token);
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
