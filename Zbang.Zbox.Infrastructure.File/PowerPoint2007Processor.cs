using System.Threading;
using Aspose.Slides;
using Aspose.Slides.Util;
using System;
using System.Drawing.Imaging;
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
        public PowerPoint2007Processor(IBlobProvider blobProvider, IBlobProvider2<IPreviewContainer> blobProviderPreview, IBlobProvider2<ICacheContainer> blobProviderCache)
            : base(blobProvider, blobProviderPreview, blobProviderCache)
        {

        }

        private static void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }

        public override async Task<PreviewResult> ConvertFileToWebsitePreviewAsync(Uri blobUri, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {

            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];

            var ppt = new AsyncLazy<Presentation>(async () =>
            {
                SetLicense();
                using (var sr = await BlobProvider.DownloadFileAsync(blobUri, cancelToken).ConfigureAwait(false))
                {
                    return new Presentation(sr);
                }
            });

            var retVal = await UploadPreviewCacheToAzureAsync(blobUri, indexNum,
                i => CreateCacheFileName(blobName, i),
                async z =>
                {
                    var p = await ppt;

                    var thumbnail = p.Slides[z].GetThumbnail(1f, 1f);
                    var ms = new MemoryStream();

                    thumbnail.Save(ms, ImageFormat.Jpeg);
                    return ms;
                }, CacheVersion, "image/jpg", cancelToken
            ).ConfigureAwait(false);
            if (ppt.IsValueCreated)
            {
                ppt.Value.Dispose();
            }
            return new PreviewResult { Content = retVal, ViewName = "Image" };
        }

        protected static string CreateCacheFileName(string blobName, int index)
        {
            return
                $"{Path.GetFileNameWithoutExtension(blobName)}{CacheVersion}_{index}_{Path.GetExtension(blobName)}.jpg";
        }


        public static readonly string[] PowerPoint2007Extensions =
        {
          ".ppt",".pot", ".pps", ".pptx", ".potx", ".ppsx", ".odp", ".pptm"
        };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.StorageContainerUrl))
            {
                return PowerPoint2007Extensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;

        }

        public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            try
            {
                var path = await BlobProvider.DownloadToLocalDiskAsync(blobUri, cancelToken).ConfigureAwait(false);
                SetLicense();
                using (var pptx = new Presentation(path))
                {
                    return await ProcessFileAsync(blobUri, () =>
                        {
                            using (var img = pptx.Slides[0].GetThumbnail(1, 1))
                            {
                                var ms = new MemoryStream();
                                img.Save(ms, ImageFormat.Jpeg);
                                return ms;
                            }

                        }
                        , () => pptx.Slides.Count, CacheVersion, cancelToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile powerpoint2007", ex);
                return null;
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


        public override async Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            var path = await BlobProvider.DownloadToLocalDiskAsync(blobUri, cancelToken).ConfigureAwait(false);
            SetLicense();
            try
            {
                using (var pptx = new Presentation(path))
                {
                    return ExtractStringFromPpt(pptx);
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("failed to initialize ppt on blob: " + blobUri, ex);
                return null;
            }
        }

    }
}
