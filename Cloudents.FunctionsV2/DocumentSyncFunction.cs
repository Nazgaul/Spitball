using Cloudents.FunctionsV2.Sync;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Cloudents.FunctionsV2
{
    public static class DocumentSyncFunction
    {
        [FunctionName("DocumentSearchSync")]
        public static async Task RunDocumentSearchAsync([TimerTrigger("0 10,40 * * * *")]
            TimerInfo timer,
            [DurableClient]IDurableOrchestrationClient starter,
            ILogger log)
        {
            await SyncFunc.StartSearchSync(starter, log, SyncType.Document);
        }
    }
}