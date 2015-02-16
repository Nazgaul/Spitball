using System.Threading;
using Aspose.Slides;
using Aspose.Slides.Util;
using ImageResizer;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class PowerPoint2007Processor : DocumentProcessor
    {
        const string CacheVersion = CacheVersionPrefix + "4";
        public PowerPoint2007Processor(IBlobProvider blobProvider)
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

            var ppt = new AsyncLazy<Presentation>(async () =>
            {
                SetLicense();
                using (var sr = await BlobProvider.DownloadFileAsync(blobName, cancelToken))
                {
                    return new Presentation(sr);
                }
            });

            var retVal = await UploadPreviewToAzure(blobName, indexNum, 
                i => CreateCacheFileName(blobName, i),
               async z =>
               {
                   var p = await ppt;

                   var thumbnail = p.Slides[z].GetThumbnail(1f, 1f);
                   var ms = new MemoryStream();

                   thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                   return ms;
               }, CacheVersion,"image/jpg"
            );
            if (ppt.IsValueCreated)
            {
                ppt.Value.Dispose();
            }
            return new PreviewResult { Content = retVal, ViewName = "Image" };
        }

        protected string CreateCacheFileName(string blobName, int index)
        {
            return string.Format("{0}{3}_{2}_{1}.jpg", Path.GetFileNameWithoutExtension(blobName), Path.GetExtension(blobName), index, CacheVersion);
        }


        public static readonly string[] PowerPoint2007Extensions =
        {
          ".ppt",".pot", ".pps", ".pptx", ".potx", ".ppsx", ".odp", ".pptm"   
        };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl))
            {
                return PowerPoint2007Extensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;

        }

        public override async Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                var path = await BlobProvider.DownloadToFileAsync(blobName, cancelToken);
                SetLicense();
                using (var pptx = new Presentation(path))
                {
                    return await ProcessFile(blobName, () =>
                    {
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

                            var output = new MemoryStream();

                            ImageBuilder.Current.Build(img, output, settings);
                            return output;
                        }

                    }, () => ExtractStringFromPpt(pptx)
                    , () => pptx.Slides.Count, CacheVersion);
                    //using (var img = pptx.Slides[0].GetThumbnail(1, 1))
                    //{

                    //    var settings = new ResizeSettings
                    //    {
                    //        Scale = ScaleMode.UpscaleCanvas,
                    //        Anchor = ContentAlignment.MiddleCenter,
                    //        BackgroundColor = Color.White,
                    //        Mode = FitMode.Crop,
                    //        Width = ThumbnailWidth,
                    //        Height = ThumbnailHeight,
                    //        Quality = 80,
                    //        Format = "jpg"
                    //    };

                    //    // ImageResizer.ImageBuilder.Current.Build(img, outputFileName + "2.jpg", settings);

                    //    using (var output = new MemoryStream())
                    //    {
                    //        ImageBuilder.Current.Build(img, output, settings);
                    //        var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) +
                    //                                      ".thumbnailV3.jpg";
                    //        var t1 = BlobProvider.UploadFileThumbnailAsync(thumbnailBlobAddressUri, output, "image/jpeg", cancelToken);
                    //        var pptContent = ExtractStringFromPpt(pptx);

                    //        var t2 = UploadMetaData(pptContent, blobName);

                    //        await Task.WhenAll(t1, t2);
                    //        return new PreProcessFileResult
                    //        {
                    //            ThumbnailName = thumbnailBlobAddressUri,
                    //            FileTextContent = pptContent
                    //        };
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile powerpoint2007", ex);
                return new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() };
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
                        }


                return StripUnwantedChars(sb.ToString());
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Filed to extract text", ex);
                return string.Empty;
            }
        }

        public override string GetDefaultThumbnailPicture()
        {
            return PowerPointFileTypePicture;
        }

        public override async Task<string> ExtractContent(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = GetBlobNameFromUri(blobUri);
                var path = await BlobProvider.DownloadToFileAsync(blobName, cancelToken);
                SetLicense();
            using (var pptx = new Presentation(path))
            {
               return ExtractStringFromPpt(pptx);
            }
        }
    }
}
