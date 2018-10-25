using System.Threading;
using System.Threading.Tasks;
using Cloudents.FunctionsV2.Sync;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Cloudents.FunctionsV2
{
    public static class UniversityFunction
    {
        [FunctionName("UniversitySearchSync")]
        public static async Task RunQuestionSearchAsync([TimerTrigger("0 */30 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation("UniversitySearchSync started");
            await SyncFunc.StartSearchSync(starter, log, SyncType.University);
            log.LogInformation("UniversitySearchSync ended");

        }
    }
}
