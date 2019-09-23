using System.Threading;
using System.Threading.Tasks;
using Cloudents.FunctionsV2.Binders;
using Cloudents.Search.Document;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2
{
    public static class DocumentFunctionBlobScan
    {
        [FunctionName("BlobFunction")]
        public static async Task RunAsync(
            [BlobTrigger("spitball-files/files/{id}/text.txt")]string text, long id,
            [Queue("generate-search-preview")] IAsyncCollector<string> collector,
            [AzureSearchSync(DocumentSearchWrite.IndexName)]  IAsyncCollector<AzureSearchSyncOutput> indexInstance,
            CancellationToken token)
        {
            await collector.AddAsync(id.ToString(), token);
        }
    }
}