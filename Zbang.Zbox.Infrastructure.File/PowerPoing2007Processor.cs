using System.Globalization;
using System.Threading;
using Aspose.Slides;
using Aspose.Slides.Util;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class PowerPoing2007Processor : FileProcessor
    {
        const string CacheVersion = "V4";
        public PowerPoing2007Processor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }

        private static void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }

        public async override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            var indexOfPageGenerate = CalculateTillWhenToDrawPictures(indexNum);

            var ppt = new Lazy<Presentation>(() =>
            {
                SetLicense();
                using (var sr = BlobProvider.DownloadFile(blobName))
                {
                    return new Presentation(sr);
                }
            });
            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task<string>>();
            var tasks = new List<Task>();

            var meta = await BlobProvider.FetechBlobMetaDataAsync(blobName);
            for (var pageIndex = indexNum; pageIndex < indexOfPageGenerate; pageIndex++)
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

                //var cacheBlobNameWithSharedAccessSignature = m_BlobProvider.GenerateSharedAccressReadPermissionInCache(cacheblobName, 20);
                //if (!string.IsNullOrEmpty(cacheBlobNameWithSharedAccessSignature))
                //{
                //    blobsNamesInCache.Add(cacheBlobNameWithSharedAccessSignature);
                //    continue;
                //}
                try
                {

                    var retVal = ppt.Value.Slides[pageIndex].GetThumbnail(1f, 1f);
                    using (var ms = new MemoryStream())
                    {
                        retVal.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        var compressor = new Compress();
                        var gzipSr = compressor.CompressToGzip(ms);
                        parallelTask.Add(BlobProvider.UploadFileToCacheAsync(cacheblobName, gzipSr, "image/jpg", true));
                        meta.Add(metaDataKey, DateTime.UtcNow.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture));
                        //blobsNamesInCache.Add(cacheName);
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
            if (ppt.IsValueCreated)
            {
                ppt.Value.Dispose();
            }
            return new PreviewResult { Content = blobsNamesInCache, ViewName = "Image" };
        }

        protected string CreateCacheFileName(string blobName, int index)
        {
            return string.Format("{0}{3}_{2}_{1}.jpg", Path.GetFileNameWithoutExtension(blobName), Path.GetExtension(blobName), index, CacheVersion);
        }


        public static readonly string[] PowerPoint2007Extenstions = { ".pptx", ".potx", ".ppxs", ".ppsx", ".ppt", ".pot", ".pps" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl))
            {
                return PowerPoint2007Extenstions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;

        }

        public override Task<PreProcessFileResult> PreProcessFile(Uri blobUri)
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                Presentation pptx;

                using (var stream = BlobProvider.DownloadFile(blobName))
                {
                    SetLicense();
                    pptx = new Presentation(stream);
                   
                }

                using (var img = pptx.Slides[0].GetThumbnail(1, 1))
                {

                    var settings = new ResizeSettings
                    {
                        Scale = ScaleMode.UpscaleCanvas,
                        Anchor = ContentAlignment.MiddleCenter,
                        BackgroundColor = Color.White,
                        Mode = FitMode.Crop,
                        Width = ThumbnailWidth,
                        Height = ThumbnailHeight,
                        Quality = 80,
                        Format = "jpg"
                    };

                    // ImageResizer.ImageBuilder.Current.Build(img, outputFileName + "2.jpg", settings);

                    using (var output = new MemoryStream())
                    {
                        ImageBuilder.Current.Build(img, output, settings);
                        var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
                        BlobProvider.UploadFileThumbnail(thumbnailBlobAddressUri, output, "image/jpeg");
                        var pptContent =  ExtractStringFromPpt(pptx);
                        pptx.Dispose();
                        return Task.FromResult(new PreProcessFileResult
                        {
                            ThumbnailName = thumbnailBlobAddressUri,
                            FileTextContent = pptContent
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile powerpoint2007", ex);
                return Task.FromResult(new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() });
            }
        }
        private string ExtractStringFromPpt(Presentation ppt)
        {
            try
            {
                var sb = new StringBuilder();
                var textFramesSlideOne = SlideUtil.GetAllTextFrames(ppt, false);

                //Loop through the Array of TextFrames
                foreach (ITextFrame t in textFramesSlideOne)
                    foreach (var para in t.Paragraphs)

                        //Loop through portions in the current Paragraph
                        foreach (var port in para.Portions)
                        {
                            //Display text in the current portion
                            sb.Append(port.Text);

                            //Display font height of the text
                            //Console.WriteLine(port.PortionFormat.FontHeight);

                            //Display font name of the text
                            //Console.WriteLine(port.PortionFormat.LatinFont.FontName);
                        }

                var str = sb.ToString().Replace("‏אזהרה‏ הנך רשאי להשתמש ' שימוש הוגן ' ביצירה מוגנת למטרות שונות, לרבות ' לימוד עצמי ' ואין לעשות שימוש בעל אופי מסחרי או מעין-מסחרי בסיכומי הרצאות תוך פגיעה בזכות היוצר של המרצה, שעמל על הכנת ההרצאות והחומר לציבור התלמידים.", string.Empty);
                str = Regex.Replace(str, @"\s+", " ");
                return str.Substring(0, Math.Min(400, str.Length));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Filed to extract text", ex);
                return string.Empty;
            }
        }

        public override string GetDefaultThumbnailPicture()
        {
            return Thumbnail.ThumbnailProvider.PowerPointFileTypePicture;
        }
    }
}
