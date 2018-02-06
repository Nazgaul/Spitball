using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Function
{
    public static class FunctionHost
    {
        static ServiceLocator _locator = new ServiceLocator();
        [FunctionName("KeepAlive")]
        public static void KeepAlive([TimerTrigger("0 */4 * * * *",RunOnStartup = true)]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }


        [FunctionName("UrlRedirect")]
        public static async Task ProcessLinksAsync([QueueTrigger(QueueName.UrlRedirectName)] UrlRedirectQueueMessage content, TraceWriter log, CancellationToken token)
        {
            var bus = _locator.Instance.Resolve<ICommandBus>();
            var command = new CreateUrlStatsCommand(content.Host, content.DateTime, content.Url, content.UrlReferrer,
                content.Location);

            await bus.DispatchAsync(command, token).ConfigureAwait(false);
            log.Info("finish processing click");
        }
    }
}
