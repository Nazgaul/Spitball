using System.Globalization;
using System.Threading;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Aspose.Pdf.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class PdfProcessor : FileProcessor
    {

        const string CacheVersion = "V4";
        public PdfProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }


        private void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }



        public async override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            var indexOfPageGenerate = CalculateTillWhenToDrawPictures(indexNum);

            Stream blobSr = null;

            var pdf = new Lazy<Document>(() =>
            {
                SetLicense();
                blobSr = BlobProvider.DownloadFile(blobName);
                return new Document(blobSr);
            });
            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task<string>>();
            var tasks = new List<Task>();


            var meta = await BlobProvider.FetechBlobMetaDataAsync(blobName);
            for (var pageIndex = ++indexNum; pageIndex < indexOfPageGenerate; pageIndex++)
            {
                string value;
                var metaDataKey = CacheVersion + pageIndex;
                var cacheblobName = CreateCacheFileName(blobName, pageIndex);


                if (meta.TryGetValue(metaDataKey, out value))
                {
                    blobsNamesInCache.Add(BlobProvider.GenerateSharedAccressReadPermissionInCacheWithoutMeta(cacheblobName, 20));
                    meta[metaDataKey] = DateTime.UtcNow.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture);// DateTime.UtcNow.ToString();
                    continue;
                }

                try
                {
                    var resolution = new Resolution(150);
                    var jpegDevice = new JpegDevice(resolution, 90);

                    using (var ms = new MemoryStream())
                    {
                        jpegDevice.Process(pdf.Value.Pages[pageIndex], ms);
                        var compressor = new Compress();
                        var sr = compressor.CompressToGzip(ms);
                        parallelTask.Add(BlobProvider.UploadFileToCacheAsync(cacheblobName, sr, "image/jpg", true));
                        meta.Add(metaDataKey, DateTime.UtcNow.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture));
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            }
            var t = BlobProvider.SaveMetaDataToBlobAsync(blobName, meta);
            tasks.AddRange(parallelTask);
            tasks.Add(t);

            await Task.WhenAll(tasks);
            blobsNamesInCache.AddRange(parallelTask.Select(s => s.Result));

            if (pdf.IsValueCreated && blobSr != null)
            {
                blobSr.Dispose();
                pdf.Value.Dispose();
            }
            return new PreviewResult { Content = blobsNamesInCache, ViewName = "Image" };
        }
        protected string CreateCacheFileName(string blobName, int index)
        {
            return string.Format("{0}{3}_{2}_{1}.jpg", Path.GetFileNameWithoutExtension(blobName), Path.GetExtension(blobName), index, CacheVersion);
        }



        public static readonly string[] PdfExtensions = { ".pdf" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl))
            {
                return PdfExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;

        }


        public override async Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                SetLicense();
                using (var stream = await BlobProvider.DownloadFileAsync(blobName))
                {
                    using (var pdfDocument = new Document(stream))
                    {
                        var jpegDevice = new JpegDevice(ThumbnailWidth, ThumbnailHeight, new Resolution(150), 80);
                        using (var ms = new MemoryStream())
                        {
                            jpegDevice.Process(pdfDocument.Pages[1], ms);
                            var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
                            await BlobProvider.UploadFileThumbnailAsync(thumbnailBlobAddressUri, ms, "image/jpeg");

                            return new PreProcessFileResult
                            {
                                ThumbnailName = thumbnailBlobAddressUri,
                                FileTextContent = ExtractPdfText(pdfDocument)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile pdf", ex);
                return new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() };
            }
        }

        private string ExtractPdfText(Document doc)
        {
            try
            {
                var textAbsorber = new TextAbsorber();
                for (var i = 1; i < Math.Min(doc.Pages.Count, 10); i++)
                {
                    doc.Pages[i].Accept(textAbsorber);
                }
                var str = textAbsorber.Text;
                str = str.Replace("‏אזהרה‏ הנך רשאי להשתמש ' שימוש הוגן ' ביצירה מוגנת למטרות שונות, לרבות ' לימוד עצמי ' ואין לעשות שימוש בעל אופי מסחרי או מעין-מסחרי בסיכומי הרצאות תוך פגיעה בזכות היוצר של המרצה, שעמל על הכנת ההרצאות והחומר לציבור התלמידים.", string.Empty);
                str = Regex.Replace(str, @"\s+", " ");
                return str.Substring(0, Math.Min(400, str.Length));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("trying to extract pdf text", ex);
                return string.Empty;
            }
        }
        public override string GetDefaultThumbnailPicture()
        {
            return ThumbnailProvider.PdfFileTypePicture;
        }
    }
}
