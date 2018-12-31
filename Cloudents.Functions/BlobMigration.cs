using Cloudents.Functions.Di;
using Cloudents.Infrastructure.Framework;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Functions
{
    public static class BlobMigration
    {
        [FunctionName("BlobPreview")]
        public static async Task Run([BlobTrigger("spitball-files/files/{id}/file-{guid}-{name}")]
            CloudBlockBlob myBlob, string id, string name,
            [Queue("generate-blob-preview")] IAsyncCollector<string> collector,
            TraceWriter log,
            CancellationToken token)
        {
            log.Info($"pushing to queue {id}");
            await collector.AddAsync(id, token);
        }




        [FunctionName("BlobBlur")]
        public static async Task Run2([BlobTrigger("spitball-files/files/{id}/preview-{idx}.jpg")]
            CloudBlockBlob myBlob, string id, string idx,
            [Queue("generate-blob-preview-blur")] IAsyncCollector<string> collector,
            TraceWriter log, CancellationToken token)
        {
            log.Info($"pushing to queue {id}");
            await collector.AddAsync(id, token);
        }




        [FunctionName("BlobPreview-Blur-Queue")]
        public static async Task BlobPreviewQueueRun2(
            [QueueTrigger("generate-blob-preview-blur")]
            string id,
            [Inject] IBlurProcessor factory,
            [Blob("spitball-files/files/{QueueTrigger}")]
            CloudBlobDirectory directory,
            TraceWriter log, CancellationToken token)
        {
            log.Info($"receive blur for {id}");
            var tasks = new List<Task>();
            foreach (var blob in directory.ListBlobs())
            {
                var myBlob = (CloudBlockBlob)blob;
                if (Regex.IsMatch(myBlob.Name, "preview-\\d*.jpg", RegexOptions.IgnoreCase))
                {
                    var idx = Path.GetFileNameWithoutExtension(myBlob.Name.Split('-').Last());
                    var blurBlob = directory.GetBlockBlobReference($"blur-{idx}.jpg");
                    if (blurBlob.Exists())
                    {
                        continue;
                    }
                    var page = int.Parse(idx);
                    using (var ms = await myBlob.OpenReadAsync(token))
                    {

                        var t = factory.ProcessBlurPreviewAsync(ms, page == 0, stream =>
                       {
                           stream.Seek(0, SeekOrigin.Begin);

                           blurBlob.Properties.ContentType = "image/jpeg";
                           log.Info($"uploading to {id} {blurBlob.Name}");
                           return blurBlob.UploadFromStreamAsync(stream, token);
                       }, token);
                        tasks.Add(t);
                    }
                }
            }

            await Task.WhenAll(tasks);
            log.Info($"finish to  blur {id}");
        }


        [FunctionName("BlobPreview-Queue")]
        public static async Task BlobPreviewQueueRun(
            [QueueTrigger("generate-blob-preview")] string id,
            [Inject] IFactoryProcessor factory,
            [Blob("spitball-files/files/{QueueTrigger}")]CloudBlobDirectory directory,
            [Queue("generate-blob-preview-poison")] IAsyncCollector<string> collector,
            [Queue("generate-blob-preview-blur")] IAsyncCollector<string> collectorBlur,
            TraceWriter log, CancellationToken token)
        {
            log.Info($"receive preview for {id}");
            var t = await directory.ListBlobsSegmentedAsync(null, token);
            var myBlob = (CloudBlockBlob)t.Results.FirstOrDefault(f => f.Uri.Segments.Last().StartsWith("file-"));
            if (myBlob == null)
            {
                return;
            }
            var name = myBlob?.Name.Split('-').Last();


            log.Info($"Going to process - {id}");

            try
            {
                using (var ms = await myBlob.OpenReadAsync(token))
                {
                    var f = factory.PreviewFactory(name);
                    if (f != null)
                    {
                        await f.ProcessFilesAsync(ms, (stream, previewName) =>
                        {

                            stream.Seek(0, SeekOrigin.Begin);
                            var blob = directory.GetBlockBlobReference($"preview-{previewName}");
                            if (blob.Exists())
                            {
                                log.Info($"uploading to {id} preview-{previewName}");
                                //if we want to reprocess this file we need to remove this line of code.
                                return Task.CompletedTask;
                            }
                            blob.Properties.ContentType = "image/jpeg";

                            log.Info($"uploading to {id} preview-{previewName}");
                            return blob.UploadFromStreamAsync(stream, token);
                        }, (text, pageCount) =>
                        {
                            var blob = directory.GetBlockBlobReference("text.txt");
                            blob.Properties.ContentType = "text/plain";
                            text = StripUnwantedChars(text);
                            blob.Metadata["PageCount"] = pageCount.ToString();
                            return blob.UploadTextAsync(text ?? string.Empty, token);
                        }, token);
                    }
                    else
                    {
                        log.Error($"did not process id:{id}");
                        await collector.AddAsync(id, token);
                    }

                    await collectorBlur.AddAsync(id, token);
                    log.Info("C# Blob trigger function Processed");

                }
            }
            catch (Exception ex)
            {
                log.Error($"did not process id:{id}", ex);
                await collector.AddAsync(id, token);
            }


        }


        private static string StripUnwantedChars(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            input = Regex.Replace(input, "\\b(\\S)\\s+(?=\\S)", string.Empty);
            var result = Regex.Replace(input, @"\s+", " ");
            var eightOrNineDigitsId = new Regex(@"\b\d{8,9}\b", RegexOptions.Compiled);

            result = eightOrNineDigitsId.Replace(result, string.Empty);
            var sb = new StringBuilder(new string(result.Where(w => char.IsLetterOrDigit(w) || char.IsWhiteSpace(w)).ToArray()));
            sb.Replace(
              "אזהרההנך רשאי להשתמש  שימוש הוגן  ביצירה מוגנת למטרות שונותלרבות  לימוד עצמי  ואין לעשות שימוש בעל אופי מסחרי או מעיןמסחרי בסיכומי הרצאות תוך פגיעה בזכות היוצר של המרצהשעמל על הכנת ההרצאות והחומר לציבור התלמידים",
                string.Empty);
            sb.Replace("בס\"ד", string.Empty);
            return sb.Replace("find more resources at oneclass.com", string.Empty).ToString();
        }



    }
}
