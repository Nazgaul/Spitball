using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Function
{
    public static class Function1
    {
        [FunctionName("KeepAlive")]
        public static void KeepAlive([TimerTrigger("0 */4 * * * *", RunOnStartup = true)]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
