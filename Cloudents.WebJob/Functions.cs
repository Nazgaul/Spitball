using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Microsoft.Azure.WebJobs;

namespace Cloudents.WebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        //public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        //{
        //    log.WriteLine(message);
        //}
        private readonly ICommandBus _commandBus;

        public Functions(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public async Task ProcessQueueMessage([QueueTrigger(QueueName.UrlRedirectName)] UrlRedirectQueueMessage content, TextWriter log, CancellationToken token)
        {
            var command = new CreateUrlStatsCommand(content.Host, content.DateTime, content.Url, content.UrlReferrer,
                content.Location, content.Ip);

            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            await log.WriteLineAsync("Finish Process").ConfigureAwait(false);
        }
    }
}
