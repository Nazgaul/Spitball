using System;
using Cloudents.Core.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Function
{
    public static class FunctionHost
    {
        [FunctionName("KeepAlive")]
        public static void Run([TimerTrigger("0 */4 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info("Keep-Alive invoked.");
        }

        [FunctionName("Url-Redirect")]
        public static void Run([QueueTrigger(QueueName.UrlRedirectName)] byte[] content)

        {

        }
    }
}
