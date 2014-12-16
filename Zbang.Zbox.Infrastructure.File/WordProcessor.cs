using System.Globalization;
using System.Net;
using System.Threading;
using Aspose.Words;
using Aspose.Words.Saving;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class WordProcessor : FileProcessor
    {
        const string VersionCache = "V6";
        public WordProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {
            SetLicense();
        }

        private void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }

        public async override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = GetBlobNameFromUri(blobUri);
            var indexOfPageGenerate = CalculateTillWhenToDrawPictures(indexNum);

            var word = new AsyncLazy<Document>(async () =>
            {
                SetLicense();
                using (var sr = await BlobProvider.DownloadFileAsync(blobName))
                {
                    return new Document(sr);
                }
            });

            var meta = await BlobProvider.FetechBlobMetaDataAsync(blobName);

            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task<string>>();
            var tasks = new List<Task>();

            var svgOptions = new SvgSaveOptions { ShowPageBorder = false, FitToViewPort = true, JpegQuality = 85, ExportEmbeddedImages = true, PageCount = 1 };
            for (var pageIndex = indexNum; pageIndex < indexOfPageGenerate; pageIndex++)
            {
                string value;
                var cacheblobName = CreateCacheFileName(blobName, pageIndex);

                var metaDataKey = VersionCache + pageIndex;
                if (meta.TryGetValue(metaDataKey, out value))
                {
                    blobsNamesInCache.Add(BlobProvider.GenerateSharedAccressReadPermissionInCacheWithoutMeta(cacheblobName, 20));
                    meta[metaDataKey] = DateTime.UtcNow.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture);
                    continue;
                }
                svgOptions.PageIndex = pageIndex;
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        var w = await word;
                        w.Save(ms, svgOptions);
                        var compressor = new Compress();
                        var sr = compressor.CompressToGzip(ms);
                        parallelTask.Add(BlobProvider.UploadFileToCacheAsync(cacheblobName, sr, "image/svg+xml", true));
                        meta.Add(metaDataKey, DateTime.UtcNow.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture));
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }

            }
            var t = BlobProvider.SaveMetaDataToBlobAsync(blobName, meta);
            tasks.AddRange(parallelTask);
            tasks.Add(t);
            await Task.WhenAll(tasks);
            blobsNamesInCache.AddRange(parallelTask.Select(s => s.Result));

            return new PreviewResult { Content = blobsNamesInCache, ViewName = "Svg" };
        }
        protected string CreateCacheFileName(string blobName, int index)
        {
            return string.Format("{0}{3}_{2}_{1}.svg", Path.GetFileNameWithoutExtension(blobName), Path.GetExtension(blobName), index, VersionCache);
        }

        public static readonly string[] WordExtensions = { ".rtf", ".docx", ".doc", ".odt" };
        public override bool CanProcessFile(Uri blobName)
        {
            return blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl) && WordExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        }

        public override async Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                Document word;

                using (var sr = await BlobProvider.DownloadFileAsync(blobName, cancelToken))
                {
                    SetLicense();
                    word = new Document(sr);

                }

                var imgOptions = new ImageSaveOptions(SaveFormat.Jpeg)
                {
                    JpegQuality = 80,
                };
                var settings = new ResizeSettings
                {
                    Width = ThumbnailWidth,
                    Height = ThumbnailHeight,
                    Quality = 80,
                    Format = "jpg"
                };

                using (var ms = new MemoryStream())
                {
                    word.Save(ms, imgOptions);
                    ms.Seek(0, SeekOrigin.Begin);
                    using (var output = new MemoryStream())
                    {
                        ImageBuilder.Current.Build(ms, output, settings);
                        var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
                        await BlobProvider.UploadFileThumbnailAsync(thumbnailBlobAddressUri, output, "image/jpeg", cancelToken);
                        return new PreProcessFileResult
                        {
                            ThumbnailName = thumbnailBlobAddressUri,
                            FileTextContent = ExtractDocumentText(word)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile word", ex);
                return new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() };
            }
        }

        private string ExtractDocumentText(Document doc)
        {
            try
            {
                var str = doc.ToString(SaveFormat.Text);

                str = str.Replace("‏אזהרה‏ הנך רשאי להשתמש ' שימוש הוגן ' ביצירה מוגנת למטרות שונות, לרבות ' לימוד עצמי ' ואין לעשות שימוש בעל אופי מסחרי או מעין-מסחרי בסיכומי הרצאות תוך פגיעה בזכות היוצר של המרצה, שעמל על הכנת ההרצאות והחומר לציבור התלמידים.", string.Empty);
                str = Regex.Replace(str, @"\s+", " ");
                str = WebUtility.HtmlEncode(str);
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
            return ThumbnailProvider.WordFileTypePicture;
        }



    }
}
