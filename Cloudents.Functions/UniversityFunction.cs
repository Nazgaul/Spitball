using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Functions.Sync;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Functions
{
    public static class UniversityFunction
    {
        [FunctionName("UniversitySearchSync")]
        public static async Task RunQuestionSearchAsync([TimerTrigger("0 */30 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            [OrchestrationClient] DurableOrchestrationClient starter,
            TraceWriter log,
            CancellationToken token)
        {
            const string instanceId = "UniversitySearchSync";
            await SyncFunc.StartSearchSync(starter, log, instanceId);
           
        }
    }
}
