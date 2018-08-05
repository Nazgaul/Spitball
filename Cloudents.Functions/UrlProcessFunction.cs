using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Cloudents.Functions.Di;
using JetBrains.Annotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Functions
{
    public static class UrlProcessFunction
    {
        //[FunctionName("UrlProcess")]
        //[UsedImplicitly]
        //public static async Task ProcessQueueMessageAsync([QueueTrigger(QueueName.UrlRedirectName)] UrlRedirectQueueMessage content,
        //    TraceWriter log, [Inject] ICommandBus commandBus, CancellationToken token)
        //{
        //    await ProcessQueueAsync(content, log, commandBus, token).ConfigureAwait(false);
        //}

        //[FunctionName("UrlProcessPoison")]
        //[UsedImplicitly]
        //public static async Task ProcessQueueMessagePoisonAsync([QueueTrigger(QueueName.UrlRedirectName + "-poison")] UrlRedirectQueueMessage content,
        //    TraceWriter log, [Inject] ICommandBus commandBus, CancellationToken token)
        //{
        //    await ProcessQueueAsync(content, log, commandBus, token).ConfigureAwait(false);
        //}


        [FunctionName("UrlProcessServiceBus")]
        public static async Task BlockChainQnaAsync(
            [ServiceBusTrigger(TopicSubscription.Background, nameof(TopicSubscription.UrlRedirect))]
            UrlRedirectQueueMessage content,
            TraceWriter log, [Inject] ICommandBus commandBus,
            CancellationToken token)
        {
            if (content == null)
            {
                log.Warning("got null message");
                return;
            }
            await ProcessQueueAsync(content, log, commandBus, token).ConfigureAwait(false);
            //if (obj.DeliveryCount > 3)
            //{
            //    return;
            //}
            //var qnaObject = obj.GetBodyInheritance<BlockChainQnaSubmit>();
            //await service.SubmitAsync((dynamic)qnaObject, token).ConfigureAwait(false);
            //log.Info("success");
        }

        private static async Task ProcessQueueAsync(UrlRedirectQueueMessage content, TraceWriter log, ICommandBus commandBus,
            CancellationToken token)
        {
            log.Info("Getting Url process message " + content);
            var command = new CreateUrlStatsCommand(content.Host, content.DateTime, content.Url.AbsoluteUri, content.UrlReferrer,
                content.Location, content.Ip);

            await commandBus.DispatchAsync(command, token).ConfigureAwait(false);
        }
    }
}
