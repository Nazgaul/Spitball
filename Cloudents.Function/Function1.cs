using System;
using System.Net.Http;
using Cloudents.Core.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Function
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        [FunctionName("Url-Redirect")]
        public static void Run([QueueTrigger(QueueName.UrlRedirectName)] byte[] content)

        {

        }
    }
}
