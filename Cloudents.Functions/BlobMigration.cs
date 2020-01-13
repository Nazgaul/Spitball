using Cloudents.Functions.Di;
using Cloudents.Infrastructure.Framework;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Functions
{
    public static class BlobMigration
    {

        [FunctionName("BlobPreviewGenerator")]
        public static async Task BlobPreviewQueueRun(
            //[QueueTrigger("generate-blob-preview", Connection = "LocalStorage")] string id,
            [QueueTrigger("generate-blob-preview")] string id,
            [Inject] IFactoryProcessor factory,
            [Blob("spitball-files/files/{QueueTrigger}")]CloudBlobDirectory directory,
            [Queue("generate-search-preview")] IAsyncCollector<string> collectorSearch,
            [Queue("generate-blob-preview-v2")] IAsyncCollector<string> collectorPreview2,
            TraceWriter log, CancellationToken token)
        {
            try
            {
                log.Info($"receive preview for {id}");
                var segment = await directory.ListBlobsSegmentedAsync(null, token);
                var originalBlob = (CloudBlockBlob)segment.Results.FirstOrDefault(f2 => f2.Uri.Segments.Last().StartsWith("file-"));
                if (originalBlob == null)
                {
                    return;
                }
                var f = factory.PreviewFactory(originalBlob.Name);
                if (f is null)
                {
                    await collectorPreview2.AddAsync(id, token);
                    log.Error($"did not process id:{id}");
                    return;
                }

                var lease = await originalBlob.AcquireLeaseAsync(TimeSpan.FromMinutes(1));
                var accessCondition = AccessCondition.GenerateLeaseCondition(lease);
                await originalBlob.FetchAttributesAsync(accessCondition, null, null, token);
                const string cantProcess = "CantProcess2";
                if (originalBlob.Metadata.TryGetValue(cantProcess, out var s) && bool.TryParse(s, out var b) && b)
                {
                    log.Error($"aborting process CantProcess attribute - {id}");
                    return;
                }

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


                using (var wait = new ManualResetEventSlim(false))
                {
                    var work = new Thread(async () =>
                    {
                        var z = Path.Combine(Path.GetTempPath(), id);
                        Stream sr = null;
                        try
                        {
                            f.Init(() =>
                            {
                                originalBlob.DownloadToFile(z, FileMode.Create, accessCondition, new BlobRequestOptions()
                                {
                                    DisableContentMD5Validation = true
                                });
                                sr = File.Open(z, FileMode.Open);
                                return sr;
                            });
                            var pageCount = 0;

                            const string blobTextName = "text.txt";
                            if (segment.Results.FirstOrDefault(d =>
                                    d.Uri.Segments.Last().StartsWith(blobTextName)) != null)
                            {
                                var blob = directory.GetBlockBlobReference(blobTextName);
                                await blob.FetchAttributesAsync(token);
                                blob.Metadata.TryGetValue("PageCount", out var pageCountStr);
                                int.TryParse(pageCountStr, out pageCount);
                            }

                            if (pageCount == 0)
                            {
                                log.Info($"Need to extract text and get page count for id:{id}");
                                var (text, pagesCount) = f.ExtractMetaContent();
                                var blob = directory.GetBlockBlobReference(blobTextName);
                                blob.Properties.ContentType = "text/plain";
                                text = StripUnwantedChars(text);
                                blob.Metadata["PageCount"] = pagesCount.ToString();
                                await blob.UploadTextAsync(text ?? string.Empty, token);
                                await collectorSearch.AddAsync(id, token);
                                pageCount = pagesCount;
                            }

                            if (pageCount != previewDelta.Count || previewDelta.Count == 0)
                            {
                                log.Info($"Processing images for id:{id}");
                                await f.ProcessFilesAsync(previewDelta, (stream, previewName) =>
                                {
                                    workHasBeenDone = true;
                                    stream.Seek(0, SeekOrigin.Begin);
                                    var blob = directory.GetBlockBlobReference($"preview-{previewName}");
                                    blob.Properties.ContentType = "image/jpeg";
                                    return blob.UploadFromStreamAsync(stream, token);
                                }, token);
                            }


                            // ReSharper disable once AccessToDisposedClosure [InstantHandleAttribute]
                            wait.Set();
                        }
                        catch (PreviewFailedException e)
                        {
                            originalBlob.Metadata[cantProcess] = true.ToString();
                            originalBlob.Metadata["ErrorProcess"] = e.Message;

                            await originalBlob.SetMetadataAsync(token);
                            log.Error($"did not process id:{id}", e);
                            // ReSharper disable once AccessToDisposedClosure [InstantHandleAttribute]
                            wait.Set();
                        }
                        catch (Exception ex)
                        {
                            if (ex.GetType().Namespace?.StartsWith("aspose", StringComparison.OrdinalIgnoreCase) == true ||
                                ex.Source.StartsWith("aspose",
                                    StringComparison.OrdinalIgnoreCase))
                            {
                                originalBlob.Metadata[cantProcess] = true.ToString();

                            }
                            originalBlob.Metadata["ErrorProcess"] = ex.Message;
                            await originalBlob.SetMetadataAsync(accessCondition, null, null, token);
                            log.Error($"did not process id:{id}", ex);
                            // ReSharper disable once AccessToDisposedClosure [InstantHandleAttribute]
                            wait.Set();
                        }
                        finally
                        {
                            sr?.Dispose();
                            if (File.Exists(z))
                            {
                                File.Delete(z);
                            }
                        }
                    });
                    work.Start();

                    var signal = wait.Wait(TimeSpan.FromMinutes(9));
                    if (!signal)
                    {
                        work.Abort();
                        if (!workHasBeenDone)
                        {
                            originalBlob.Metadata[cantProcess] = true.ToString();
                            await originalBlob.SetMetadataAsync(token);
                        }

                        log.Error($"aborting process - {id}");
                    }
                }

                await originalBlob.ReleaseLeaseAsync(accessCondition, token);


            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
                    log.Warning("another process is processing the file");
                else
                    log.Error($"did not process id:{id}", ex);
            }
            catch (Exception ex)
            {
                log.Error($"did not process id:{id}", ex);
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
