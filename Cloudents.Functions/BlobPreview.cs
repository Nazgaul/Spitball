using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Functions
{
    public static class BlobPreview
    {
        [FunctionName("BlobPreview")]
        [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting" ,Justification = "Entry point")]
        public static async Task RunAsync([BlobTrigger("spitball-files/files/{id}/file-{guid}-{name}")]
            CloudBlockBlob myBlob, string id, string name,
            [Queue("generate-blob-preview")] IAsyncCollector<string> collector,
            TraceWriter log,
            CancellationToken token)
        {
            log.Info($"pushing to queue {id}");
            await collector.AddAsync(id, token);
        }
    }
}