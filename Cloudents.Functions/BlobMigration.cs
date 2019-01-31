﻿using Cloudents.Functions.Di;
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
                if (token.IsCancellationRequested)
                {
                    break;
                }
                var myBlob = (CloudBlockBlob)blob;
                if (Regex.IsMatch(myBlob.Name, "preview-\\d*.jpg", RegexOptions.IgnoreCase))
                {
                    var idx = Path.GetFileNameWithoutExtension(myBlob.Name.Split('-').Last());
                    if (idx == null)
                    {
                        continue;
                    }
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
            try
            {
                log.Info($"receive preview for {id}");
                var segment = await directory.ListBlobsSegmentedAsync(null, token);
                var myBlob = (CloudBlockBlob)segment.Results.FirstOrDefault(f2 => f2.Uri.Segments.Last().StartsWith("file-"));
                if (myBlob == null)
                {
                    return;
                }
                var name = myBlob.Name.Split('-').Last();

                myBlob.FetchAttributes();
                if (myBlob.Metadata.TryGetValue("CantProcess", out var s) && bool.TryParse(s, out var b) && b)
                {
                    log.Error($"aborting process CantProcess attribute - {id}");
                    return;
                }


                const string contentType = "text/plain";


                var document = segment.Results.Where(w => w.Uri.Segments.Last().StartsWith("preview-"))
                    .OrderBy(o => o.Uri, new OrderPreviewComparer());

                var previewDelta = new List<int>();
                var workHasBeenDone = false;
                foreach (var item in document)
                {
                    int.TryParse(
                        Path.GetFileNameWithoutExtension(item.Uri.ToString())
                        .Split('-').Last(), out var temp
                        );
                    previewDelta.Add(temp);
                    workHasBeenDone = true;
                }

                log.Info($"Going to process - {id}");



                var f = factory.PreviewFactory(name);
                if (f != null)
                {
                    //var wait = new ManualResetEvent(false);
                    using (var wait = new ManualResetEventSlim(false))
                    {

                        //wait2.Wait()
                        var work = new Thread(async () =>
                                        {
                                            try
                                            {
                                                using (var ms = await myBlob.OpenReadAsync(token))
                                                {
                                                    f.Init(ms);
                                                    int pageCount;

                                                    const string blobTextName = "text.txt";
                                                    if (segment.Results.FirstOrDefault(d =>
                                                            d.Uri.Segments.Last().StartsWith(blobTextName)) == null)
                                                    {
                                                        var (text, pagesCount) = f.ExtractMetaContent();
                                                        var blob = directory.GetBlockBlobReference(blobTextName);
                                                        blob.Properties.ContentType = contentType;
                                                        text = StripUnwantedChars(text);
                                                        blob.Metadata["PageCount"] = pagesCount.ToString();
                                                        await blob.UploadTextAsync(text ?? string.Empty, token);
                                                        pageCount = pagesCount;
                                                    }
                                                    else
                                                    {
                                                        var blob = directory.GetBlockBlobReference(blobTextName);
                                                        await blob.FetchAttributesAsync(token);
                                                        pageCount = int.Parse(blob.Metadata["PageCount"]);
                                                    }

                                                    if (pageCount != previewDelta.Count || previewDelta.Count == 0)
                                                    {
                                                        await f.ProcessFilesAsync(previewDelta, (stream, previewName) =>
                                                        {
                                                            workHasBeenDone = true;
                                                            stream.Seek(0, SeekOrigin.Begin);
                                                            var blob = directory.GetBlockBlobReference($"preview-{previewName}");
                                                            blob.Properties.ContentType = "image/jpeg";
                                                            log.Info($"uploading to {id} preview-{previewName}");
                                                            return blob.UploadFromStreamAsync(stream, token);
                                                        }, token);
                                                    }
                                                }

                                                wait.Set();
                                            }
                                            catch (Exception ex)
                                            {
                                                wait.Set();
                                                log.Error($"did not process id:{id}", ex);

                                                await collector.AddAsync(id, token);
                                            }
                                        });
                        work.Start();

                        var signal = wait.Wait(TimeSpan.FromMinutes(9), token);
                        //var signal = wait.WaitOne(TimeSpan.FromMinutes(9));
                        if (!signal)
                        {
                            work.Abort();
                            if (!workHasBeenDone)
                            {
                                myBlob.Metadata["CantProcess"] = true.ToString();
                                await myBlob.SetMetadataAsync(token);
                            }

                            log.Error($"aborting process - {id}");
                        }
                    }
                }
                else
                {
                    log.Error($"did not process id:{id}");
                    await collector.AddAsync(id, token);
                }

                await collectorBlur.AddAsync(id, token);
                log.Info("C# Blob trigger function Processed");

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
    public class OrderPreviewComparer : IComparer<Uri>
    {
        public int Compare(Uri s1, Uri s2)
        {

            string GetNumberStr(Uri x)
            {
                return Regex.Replace(x?.Segments.Last() ?? string.Empty, "[^\\d]", string.Empty);
            }

            var z = GetNumberStr(s1);
            var z2 = GetNumberStr(s2);

            if (int.TryParse(z, out var i1) && int.TryParse(z2, out var i2))
            {
                if (i1 > i2) return 1;
                if (i1 < i2) return -1;
                if (i1 == i2) return 0;
            }

            return 0;
            //return string.Compare(s1, s2, true);
        }

    }
}
