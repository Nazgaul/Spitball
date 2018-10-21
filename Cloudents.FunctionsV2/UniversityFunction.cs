using System.Threading;
using System.Threading.Tasks;
using Cloudents.FunctionsV2.Sync;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Cloudents.FunctionsV2
{
    public static class UniversityFunction
    {
        [FunctionName("UniversitySearchSync")]
        public static async Task RunQuestionSearchAsync([TimerTrigger("0 */30 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log,
            CancellationToken token)
        {
            await SyncFunc.StartSearchSync(starter, log, SyncType.University);
           
        }
    }
}
