using Cloudents.Core.Command;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.FunctionsV2.Sync;
using Microsoft.Azure.WebJobs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class DocumentFunction
    {
        [FunctionName("BlobFunction")]
        public static async Task RunAsync(
            [BlobTrigger("spitball-files/files/{id}/text.txt")]string text, long id, IDictionary<string, string> metadata,
            [Inject] ISearchServiceWrite<Document> searchInstance,
            [Inject] ICommandBus commandBus,
            [Inject] ITextAnalysis textAnalysis,
            ILogger log, CancellationToken token)
        {
            var lang = await textAnalysis.DetectLanguageAsync(text.Truncate(5000), token);
            int? pageCount = null;
            if (metadata.TryGetValue("PageCount", out var pageCountStr) &&
                int.TryParse(pageCountStr, out var pageCount2))
            {
                pageCount = pageCount2;
            }

            var command = new UpdateDocumentMetaCommand(id, lang, pageCount);
            await commandBus.DispatchAsync(command, token);

            await searchInstance.UpdateDataAsync(new[]
                 {
                    new Document()
                    {
                        Id = id.ToString(),
                        Content = text.Truncate(6000),
                        Language = lang.TwoLetterISOLanguageName,
                        MetaContent = text.Truncate(200, true)
                    }
                }, token
             );
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
