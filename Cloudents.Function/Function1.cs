using System;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Function
{
    public static class Function1
    {
       // private static ServiceLocator _locator = new ServiceLocator();
        [FunctionName("KeepAlive")]
        public static void KeepAlive([TimerTrigger("0 */4 * * * *", RunOnStartup = true)]TimerInfo myTimer, TraceWriter log)
        {
            //var bus = _locator.Instance.GetInstance<ICommandBus>();
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
