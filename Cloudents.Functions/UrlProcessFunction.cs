using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Functions.Di;
using JetBrains.Annotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Functions
{
    public static class UrlProcessFunction
    {
        [FunctionName("UrlProcess")]
        [UsedImplicitly]
        public static async Task ProcessQueueMessageAsync([QueueTrigger(QueueName.UrlRedirectName)] UrlRedirectQueueMessage content,
            TraceWriter log, [Inject] ICommandBus commandBus, CancellationToken token)
        {
            await ProcessQueueAsync(content, log, commandBus, token).ConfigureAwait(false);
        }

        [FunctionName("UrlProcessPoison")]
        [UsedImplicitly]
        public static async Task ProcessQueueMessagePoisonAsync([QueueTrigger(QueueName.UrlRedirectName + "-poison")] UrlRedirectQueueMessage content,
            TraceWriter log, [Inject] ICommandBus commandBus, CancellationToken token)
        {
            await ProcessQueueAsync(content, log, commandBus, token).ConfigureAwait(false);
        }

        private static async Task ProcessQueueAsync(UrlRedirectQueueMessage content, TraceWriter log, ICommandBus commandBus,
            CancellationToken token)
        {
            log.Info("Getting Url process message");
            var command = new CreateUrlStatsCommand(content.Host, content.DateTime, content.Url, content.UrlReferrer,
                content.Location, content.Ip);

            await commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            log.Info("Finish Process");
        }
    }
}
