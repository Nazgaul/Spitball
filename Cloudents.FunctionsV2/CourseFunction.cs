using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.FunctionsV2.Sync;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Cloudents.FunctionsV2
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Azure function")]
    public static class CourseFunction
    {
        [FunctionName("CourseSearchSync")]
        public static async Task RunAsync([TimerTrigger("0 */30 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log,
            CancellationToken token)
        {
            //const string instanceId = "CourseSearchSync";
            await SyncFunc.StartSearchSync(starter, log, SyncType.Course);
            
        }
    }
}
