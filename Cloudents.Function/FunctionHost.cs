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

        static FunctionHost()
        {
            ApplicationHelper.Startup();
            _locator = new ServiceLocator();
        }


        private static ServiceLocator _locator;
        [FunctionName("KeepAlive")]
        public static void KeepAlive([TimerTrigger("0 */4 * * * *",RunOnStartup = true)]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }


        [FunctionName("UrlRedirect")]
        public static async Task ProcessLinksAsync([QueueTrigger(QueueName.UrlRedirectName)] UrlRedirectQueueMessage content, TraceWriter log, CancellationToken token)
        {
            var bus = _locator.Instance.GetInstance<ICommandBus>();
            var command = new CreateUrlStatsCommand(content.Host, content.DateTime, content.Url, content.UrlReferrer,
                content.Location);

            await bus.DispatchAsync(command, token).ConfigureAwait(false);
            log.Info("finish processing click");
        }
    }
}
