using Cloudents.Functions.Di;
using Cloudents.Infrastructure.Framework;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Functions
{
    public static class BlobMigration
    {
        [FunctionName("BlobPreview")]
        public static async Task Run([BlobTrigger("spitball-files/files/{id}/file-{guid}-{name}")]CloudBlockBlob myBlob, string id, string name,
            [Inject] IFactoryProcessor factory,
            [Blob("spitball-files/files/{id}")]CloudBlobDirectory directory,
            TraceWriter log, CancellationToken token)
        {

            var mimeTypeConvert = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                [".jpg"] = "image/jpeg",
                [".svg"] = "image/svg+xml"
            };
            using (var ms = new MemoryStream())
            {
                myBlob.DownloadToStream(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var f = factory.PreviewFactory(name);
                if (f != null)
                {

                    await f.ProcessFilesAsync(ms, (stream, previewName) =>
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        var blob = directory.GetBlockBlobReference($"preview-{previewName}");
                        if (previewName != null && mimeTypeConvert.TryGetValue(Path.GetExtension(previewName), out var mimeValue))
                        {
                            blob.Properties.ContentType = mimeValue;
                        }
                        else
                        {
                            log.Warning("no mime type");
                        }

                        return blob.UploadFromStreamAsync(stream, token);
                    }, s =>
                    {
                        var blob = directory.GetBlockBlobReference("text.txt");
                        blob.Properties.ContentType = "text/plain";
                        s = Regex.Replace(s, @"\s+", " ", RegexOptions.Multiline);
                        return blob.UploadTextAsync(s, token);
                    }, pageCount =>
                    {
                        var blob = directory.GetBlockBlobReference("text.txt");
                        blob.Metadata["PageCount"] = pageCount.ToString();
                        return blob.SetMetadataAsync(token);
                    }, token);
                }
                else
                {
                    log.Warning($"did not process id:{id}");
                }
                log.Info("C# Blob trigger function Processed");
            }
        }



    }
}
