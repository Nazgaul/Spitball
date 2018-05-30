using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.FunctionsV1.Di;
using JetBrains.Annotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.FunctionsV1
{
    public static class Function1
    {

        //[FunctionName("UrlProcess2")]
        //public static async Task ProcessQueueMessageAsync2([TimerTrigger("0 * * * * *")]TimerInfo myTimer,
        //    TraceWriter log, CancellationToken token, [Inject] ICommandBus commandBus)
        //{
        //    var command = new CreateUrlStatsCommand("s", DateTime.UtcNow, "s", "s",
        //        0, "s");

        //    await commandBus.DispatchAsync(command, token).ConfigureAwait(false);
        //    log.Info("Finish Process");
        //}


        [FunctionName("UrlProcess")]
        [UsedImplicitly]
        public static async Task ProcessQueueMessageAsync([QueueTrigger(QueueName.UrlRedirectName)] UrlRedirectQueueMessage content,
            TraceWriter log, CancellationToken token, [Inject] ICommandBus commandBus)
        {
            var command = new CreateUrlStatsCommand(content.Host, content.DateTime, content.Url, content.UrlReferrer,
                content.Location, content.Ip);

            await commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            log.Info("Finish Process");


        }
    }
}
