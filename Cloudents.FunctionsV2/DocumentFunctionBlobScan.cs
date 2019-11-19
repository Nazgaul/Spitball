using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2
{
    public static class DocumentFunctionBlobScan
    {
        [FunctionName("BlobFunction")]
        public static async Task RunAsync(
            [BlobTrigger("spitball-files/files/{id}/text.txt")]string text, long id,
            [Queue("generate-search-preview")] IAsyncCollector<string> collector,
            CancellationToken token)
        {
            await collector.AddAsync(id.ToString(), token);
        }
    }
}