using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Functions
{
    public static class UniversityFunction
    {
        [FunctionName("UniversityTimer")]
        public static void Run([TimerTrigger("0 */30 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        public static void ProcessUpload()
        {

        }

        public static void ProcessDelete()
        {

        }

        [FunctionName("SynonymWatch")]
        public static void SynonymWatch([BlobTrigger("spitball/Synonym/{name}")]
            string content, TraceWriter log)
        {
            log.Info(content);
        }
    }
}
