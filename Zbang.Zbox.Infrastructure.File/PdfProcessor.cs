using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Aspose.Pdf.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class PdfProcessor : FileProcessor, IContentProcessor
    {
        public PdfProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }


        private void SetLicense()
        {
            var license = new Aspose.Pdf.License();
            license.SetLicense("Aspose.Total.lic");
        }



        public async override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum)
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            var indexOfPageGenerate = CalculateTillWhenToDrawPictures(indexNum);

            Stream blobSr = null;

            var pdf = new Lazy<Document>(() =>
            {
                SetLicense();
                blobSr = m_BlobProvider.DownloadFile(blobName);
                //using (var sr = m_BlobProvider.DownloadFile(blobName))
                //{
                return new Document(blobSr);
                //}
            });// new Document(m_BlobProvider.DownloadFile(blobName));
            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task<string>>();



            for (int pageIndex = ++indexNum; pageIndex < indexOfPageGenerate; pageIndex++)
            {
                var cacheblobName = CreateCacheFileName(blobName, pageIndex);
                var cacheBlobNameWithSharedAccessSignature = m_BlobProvider.GenerateSharedAccressReadPermissionInCache(cacheblobName, 20);
                if (!string.IsNullOrEmpty(cacheBlobNameWithSharedAccessSignature))
                {
                    blobsNamesInCache.Add(cacheBlobNameWithSharedAccessSignature);
                    continue;
                }

                try
                {
                    Resolution resolution = new Resolution(150);
                    JpegDevice jpegDevice = new JpegDevice(resolution, 90);

                    using (var ms = new MemoryStream())
                    {
                        jpegDevice.Process(pdf.Value.Pages[pageIndex], ms);

                        Compress compressor = new Compress();
                        var sr = compressor.CompressToGzip(ms);

                        parallelTask.Add(m_BlobProvider.UploadFileToCacheAsync(cacheblobName, sr, "image/jpg", true));

                        //blobsNamesInCache.Add(cacheName);
                    }

                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }


                // word.Save(outputFileName + pageIndex + ".jpg", imgOptions);
            }

            await Task.WhenAll(parallelTask);
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
            return string.Format("{0}V4_{2}_{1}.jpg", Path.GetFileNameWithoutExtension(blobName), Path.GetExtension(blobName), index);
        }



        public static readonly string[] pdfExtenstions = { ".pdf" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl))
            {
                return pdfExtenstions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;

        }


        public override Task<PreProcessFileResult> PreProcessFile(Uri blobUri)
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                SetLicense();
                using (var stream = m_BlobProvider.DownloadFile(blobName))
                {
                    using (Document pdfDocument = new Document(stream))
                    {
                        JpegDevice jpegDevice = new JpegDevice(ThumbnailWidth, ThumbnailHeight, new Resolution(150), 80);
                        using (var ms = new MemoryStream())
                        {
                            jpegDevice.Process(pdfDocument.Pages[1], ms);
                            var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
                            m_BlobProvider.UploadFileThumbnail(thumbnailBlobAddressUri, ms, "image/jpeg");

                            return Task.FromResult<PreProcessFileResult>(new PreProcessFileResult
                            {
                                ThumbnailName = thumbnailBlobAddressUri,
                                FileTextContent = ExtractPdfText(pdfDocument)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile pdf", ex);
                return Task.FromResult<PreProcessFileResult>(new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() });
            }
        }

        private string ExtractPdfText(Document doc)
        {
            try
            {
                var textAbsorber = new TextAbsorber();
                doc.Pages.Accept(textAbsorber);
                var str = textAbsorber.Text;
                str = Regex.Replace(str, @"\s+", " ").Replace("אזהרה הנך רשאי להשתמש ' שימוש הוגן ' ביצירה מוגנת למטרות שונות, לרבות ' לימוד עצמי ' ואין לעשות שימוש בעל אופי מסחרי או מעין-מסחרי בסיכומי הרצאות תוך פגיעה בזכות היוצר של המרצה, שעמל על הכנת ההרצאות והחומר לציבור התלמידים.", string.Empty); 
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
            return Zbang.Zbox.Infrastructure.Thumbnail.ThumbnailProvider.PdfFileTypePicture;
        }
    }
}
