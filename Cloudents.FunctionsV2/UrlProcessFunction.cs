namespace Cloudents.FunctionsV2
{
    public static class UrlProcessFunction
    {
        //[FunctionName("UrlProcessServiceBus")]
        //public static async Task BlockChainQnaAsync(
        //    [ServiceBusTrigger(TopicSubscription.Background, nameof(TopicSubscription.UrlRedirect))]
        //    UrlRedirectQueueMessage content,
        //    ILogger log, [Inject] ICommandBus commandBus,
        //    CancellationToken token)
        //{
        //    if (content == null)
        //    {
        //        log.Warning("got null message");
        //        return;
        //    }
        //    await ProcessQueueAsync(content, log, commandBus, token).ConfigureAwait(false);
        //}

        //private static async Task ProcessQueueAsync(UrlRedirectQueueMessage content, ILogger log, ICommandBus commandBus,
        //    CancellationToken token)
        //{
        //    log.Info("Getting Url process message " + content);
        //    var command = new CreateUrlStatsCommand(content.Host, content.DateTime, content.Url.AbsoluteUri, content.UrlReferrer,
        //        content.Location, content.Ip);

        //    await commandBus.DispatchAsync(command, token).ConfigureAwait(false);
        //}
    }
}
