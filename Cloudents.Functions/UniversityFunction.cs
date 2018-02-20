using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Functions
{
    [DependencyInjectionConfig(typeof(DiConfig))]
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
        public static async Task SynonymWatch([BlobTrigger("spitball/Synonym/{name}")]
            string content, string name, TraceWriter log,
            [Inject] ISynonymWrite synonymWrite,
            CancellationToken token)
        {
            await synonymWrite.CreateOrUpdateAsync(Path.GetFileNameWithoutExtension(name), content, token);
            log.Info(content);
        }
    }
}
