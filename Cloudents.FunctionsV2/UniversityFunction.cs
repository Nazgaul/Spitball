//using Cloudents.FunctionsV2.Sync;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Extensions.Logging;
//using System.Threading.Tasks;
//using Microsoft.Azure.WebJobs.Extensions.DurableTask;

//namespace Cloudents.FunctionsV2
//{
//    public static class UniversityFunction
//    {
//        [FunctionName("UniversitySearchSync")]
//        public static async Task RunQuestionSearchAsync([TimerTrigger("0 0,30 * * * *")] TimerInfo myTimer,
//            [DurableClient]IDurableOrchestrationClient starter,
//            ILogger log)
//        {
//            log.LogInformation("UniversitySearchSync started");
//            await SyncFunc.StartSearchSync(starter, log, SyncType.University);
//            log.LogInformation("UniversitySearchSync ended");

//        }
//    }
//}
