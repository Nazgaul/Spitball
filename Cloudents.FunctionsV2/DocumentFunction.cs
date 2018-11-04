using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cloudents.FunctionsV2.Sync;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Cloudents.FunctionsV2
{
    public static class DocumentFunction
    {
        [FunctionName("BlobFunction")]
        public static void Run(
            [BlobTrigger("spitball-files/files/{id}/text.txt")]string text, IDictionary<string, string> metadata,
            string name, ILogger log)
        {

            //log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }


        [FunctionName("DocumentSearchSync")]
        public static async Task RunQuestionSearchAsync([TimerTrigger("0 */30 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            await SyncFunc.StartSearchSync(starter, log, SyncType.Document);
        }

    }
}
