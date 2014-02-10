using Aspose.Words;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using ImageResizer;
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
    public class WordProcessor : FileProcessor, IContentProcessor
    {
        public WordProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {
            SetLicense();
        }

        private void SetLicense()
        {
            var license = new Aspose.Words.License();
            license.SetLicense("Aspose.Total.lic");
        }

        public async override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum)
        {
            var blobName = GetBlobNameFromUri(blobUri);
            var indexOfPageGenerate = CalculateTillWhenToDrawPictures(indexNum);

            var word = new Lazy<Document>(() =>
            {
                SetLicense();
                using (var sr = m_BlobProvider.DownloadFile(blobName))
                {
                    return new Document(sr);
                }
            });


            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task<string>>();

            ImageSaveOptions imgOptions = new ImageSaveOptions(SaveFormat.Jpeg);
            imgOptions.JpegQuality = 80;

            for (int pageIndex = indexNum; pageIndex < indexOfPageGenerate; pageIndex++)
            {
                var cacheblobName = CreateCacheFileName(blobName, pageIndex);
                var cacheBlobNameWithSharedAccessSignature = m_BlobProvider.GenerateSharedAccressReadPermissionInCache(cacheblobName, 20);
                if (!string.IsNullOrEmpty(cacheBlobNameWithSharedAccessSignature))
                {
                    blobsNamesInCache.Add(cacheBlobNameWithSharedAccessSignature);
                    continue;
                }
                imgOptions.PageIndex = pageIndex;
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        word.Value.Save(ms, imgOptions);
                        Compress compressor = new Compress();
                        var sr = compressor.CompressToGzip(ms);
                        parallelTask.Add(m_BlobProvider.UploadFileToCacheAsync(cacheblobName, sr, "image/jpg", true));
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }

            }
            await Task.WhenAll(parallelTask);
            blobsNamesInCache.AddRange(parallelTask.Select(s => s.Result));

            return new PreviewResult { Content = blobsNamesInCache, ViewName = "Image" };
        }
        protected string CreateCacheFileName(string blobName, int index)
        {
            return string.Format("{0}V4_{2}_{1}.jpg", Path.GetFileNameWithoutExtension(blobName), Path.GetExtension(blobName), index);
        }




        public static readonly string[] wordExtenstions = { ".rtf", ".docx", ".doc", ".txt" };
        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl))
            {
                return wordExtenstions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;
        }

        public override Task<PreProcessFileResult> PreProcessFile(Uri blobUri)
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                Document word;

                using (var sr = m_BlobProvider.DownloadFile(blobName))
                {
                    SetLicense();
                    word = new Document(sr);

                }

                ImageSaveOptions imgOptions = new ImageSaveOptions(SaveFormat.Jpeg);
                imgOptions.JpegQuality = 100;

                ResizeSettings settings = new ResizeSettings();
                settings.Width = ThumbnailWidth;
                settings.Height = ThumbnailHeight;
                settings.Quality = 80;
                settings.Format = "jpg";

                using (var ms = new MemoryStream())
                {
                    word.Save(ms, imgOptions);
                    ms.Seek(0, SeekOrigin.Begin);
                    using (var output = new MemoryStream())
                    {
                        ImageResizer.ImageBuilder.Current.Build(ms, output, settings);
                        var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
                        m_BlobProvider.UploadFileThumbnail(thumbnailBlobAddressUri, output, "image/jpeg");
                        return Task.FromResult<PreProcessFileResult>(new PreProcessFileResult
                        {
                            ThumbnailName = thumbnailBlobAddressUri,
                            FileTextContent = ExtractDocumentText(word)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile word", ex);
                return Task.FromResult<PreProcessFileResult>(new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() });
            }
        }

        private string ExtractDocumentText(Document doc)
        {
            try
            {
                var str = doc.ToString(SaveFormat.Text);
                str = Regex.Replace(str, @"\s+", " ").Replace("אזהרה הנך רשאי להשתמש ' שימוש הוגן ' ביצירה מוגנת למטרות שונות, לרבות ' לימוד עצמי ' ואין לעשות שימוש בעל אופי מסחרי או מעין-מסחרי בסיכומי הרצאות תוך פגיעה בזכות היוצר של המרצה, שעמל על הכנת ההרצאות והחומר לציבור התלמידים.",string.Empty);
                return str.Substring(0, Math.Min(400, str.Length));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Failed to extract text from doc", ex);
                return string.Empty;
            }
        }

        public override string GetDefaultThumbnailPicture()
        {
            return Zbang.Zbox.Infrastructure.Thumbnail.ThumbnailProvider.WordFileTypePicture;
        }
    }
}
