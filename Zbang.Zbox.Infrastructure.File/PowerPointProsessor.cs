using Aspose.Slides;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    class PowerPointProsessor : FileProcessor, IContentProcessor
    {

        public PowerPointProsessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }
        public async override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum)
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            //indexNum = indexNum / numberOfFilesInGroup;
            //var startPage = indexNum * numberOfFilesInGroup;
            var indexOfPageGenerate = CalculateTillWhenToDrawPictures(indexNum);

            var ppt = new Lazy<Presentation>(() =>
            {
                SetLicense();
                using (var sr = m_BlobProvider.DownloadFile(blobName))
                {
                    return new Presentation(sr);
                }
            });
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
                    //if (pageIndex >= ppt.Value.Slides.Count)
                    //{
                    //    break;
                    //}
                    var slide = ppt.Value.GetSlideByPosition(pageIndex);
                    if (slide == null)
                    {
                        break;
                    }
                    var retVal = slide.GetThumbnail(1f, 1f);
                    using (var ms = new MemoryStream())
                    {
                        retVal.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        Compress compressor = new Compress();
                        var gzipSr = compressor.CompressToGzip(ms);

                        parallelTask.Add(m_BlobProvider.UploadFileToCacheAsync(cacheblobName, gzipSr, "image/jpg", true));

                        //blobsNamesInCache.Add(cacheName);
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
            return string.Format("{0}V5_{2}_{1}.jpg", Path.GetFileNameWithoutExtension(blobName), Path.GetExtension(blobName), index);
        }

        //public override byte[] ConvertFileToWebSitePreview(System.IO.Stream stream)
        //{
        //    SetLicense();

        //    var ppt = new Aspose.Slides.Presentation(stream);

        //    using (var ms = new MemoryStream())
        //    {
        //        ppt.Save(ms, Aspose.Slides.Export.SaveFormat.Pdf);
        //        return ms.ToArray();
        //    }
        //}

        private static void SetLicense()
        {
            var license = new Aspose.Slides.License();
            license.SetLicense("Aspose.Total.lic");
        }

        //public override System.Drawing.Image ConvertFileToImage(System.IO.Stream stream)
        //{
        //    SetLicense();
        //    var ppt = new Aspose.Slides.Presentation(stream);
        //    return ppt.Slides[0].GetThumbnail(1, 1);
        //}

        //public override byte[] GenerateCopyRight(Stream stream)
        //{
        //    throw new NotImplementedException();
        //}


        public static readonly string[] powerPointExtensions = { ".ppt", ".pot", ".pps" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl))
            {
                return powerPointExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;
        }

        public override Task<PreProcessFileResult> PreProcessFile(Uri blobUri)
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                Presentation ppt;
                using (var stream = m_BlobProvider.DownloadFile(blobName))
                {
                    ppt = new Aspose.Slides.Presentation(stream);
                    SetLicense();
                }

                var img = ppt.Slides[0].GetThumbnail(1, 1);


                ResizeSettings settings = new ResizeSettings();
                settings.Scale = ScaleMode.UpscaleCanvas;
                settings.Anchor = ContentAlignment.MiddleCenter;
                settings.BackgroundColor = Color.White;
                settings.Mode = FitMode.Crop;
                settings.Width = ThumbnailWidth;
                settings.Height = ThumbnailHeight;

                settings.Quality = 80;
                settings.Format = "jpg";
                // ImageResizer.ImageBuilder.Current.Build(img, outputFileName + "2.jpg", settings);

                using (var output = new MemoryStream())
                {
                    ImageResizer.ImageBuilder.Current.Build(img, output, settings);
                    var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
                    m_BlobProvider.UploadFileThumbnail(thumbnailBlobAddressUri, output, "image/jpeg");
                    return Task.FromResult<PreProcessFileResult>(new PreProcessFileResult { ThumbnailName = thumbnailBlobAddressUri });
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile powerpoint", ex);
                return Task.FromResult<PreProcessFileResult>(new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() });
            }
        }
        public override string GetDefaultThumbnailPicture()
        {
            return Zbang.Zbox.Infrastructure.Thumbnail.ThumbnailProvider.PowerPointFileTypePicture;
        }
    }
}
