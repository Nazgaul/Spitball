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
            var existingInstance = await starter.GetStatusAsync(instanceId);
            var startNewInstanceEnum = new[]
            {
                OrchestrationRuntimeStatus.Canceled,
                OrchestrationRuntimeStatus.Completed,
                OrchestrationRuntimeStatus.Failed,
                OrchestrationRuntimeStatus.Terminated
            };
            if (existingInstance == null || startNewInstanceEnum.Contains(existingInstance.RuntimeStatus))
            {
                log.Info("Started UniversitySearchSync");
                var model = new SearchSyncInput(SyncType.University);
                await starter.StartNewAsync("SearchSync", instanceId, model);
            }
        }
    }
}
