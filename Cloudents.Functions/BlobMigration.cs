using Cloudents.Functions.Di;
using Cloudents.Infrastructure.Framework;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Functions
{
    public static class BlobMigration
    {
        [FunctionName("BlobPreview")]
        public static async Task Run([BlobTrigger("spitball-files/files/{id}/{guid}-{name}")]CloudBlockBlob myBlob, string id, string name,
            [Inject] IFactoryProcessor factory,
            [Blob("spitball-files/files/{id}")]CloudBlobDirectory directory,
            TraceWriter log, CancellationToken token)
        {
            using (var ms = new MemoryStream())
            {
                myBlob.DownloadToStream(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var f = factory.PreviewFactory(name);
                if (f != null)
                {
                    await f.CreatePreviewFilesAsync(ms, (stream, previewName) =>
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        var blob = directory.GetBlockBlobReference(previewName);
                        return blob.UploadFromStreamAsync(stream, token);
                    }, s =>
                    {
                        var blob = directory.GetBlockBlobReference("text.txt");
                        blob.Properties.ContentType = "text/plain";
                        s = Regex.Replace(s, @"\s+", " ", RegexOptions.Multiline);
                        return blob.UploadTextAsync(s, token);
                    }, token);
                }

                log.Info("C# Blob trigger function Processed");
            }
        }



    }
}
