using System.Threading;
using Aspose.Imaging;
using Aspose.Imaging.FileFormats.Jpeg;
using Aspose.Imaging.FileFormats.Tiff;
using Aspose.Imaging.ImageOptions;
using Aspose.Imaging.Sources;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Image = Aspose.Imaging.Image;

namespace Zbang.Zbox.Infrastructure.File
{
    public class TiffProcessor : FileProcessor
    {
        private readonly IBlobProvider2<IPreviewContainer> m_BlobProviderPreview;
        private readonly IBlobProvider2<ICacheContainer> m_BlobProviderCache;
        private readonly ILogger m_Logger;

        public TiffProcessor(IBlobProvider blobProvider, IBlobProvider2<IPreviewContainer> blobProviderPreview, IBlobProvider2<ICacheContainer> blobProviderCache, ILogger logger)
            : base(blobProvider)
        {
            m_BlobProviderPreview = blobProviderPreview;
            m_BlobProviderCache = blobProviderCache;
            m_Logger = logger;
        }

        private static void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }
        public override async Task<PreviewResult> ConvertFileToWebsitePreviewAsync(Uri blobUri, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            Stream blobStr = null;
            var tiff = new AsyncLazy<TiffImage>(async () =>
           {
               SetLicense();
               blobStr = await BlobProvider.DownloadFileAsync(blobUri, cancelToken).ConfigureAwait(false);

               var tiffImage = (TiffImage)Image.Load(blobStr);
               return tiffImage;


           });
            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task>();
            var jpgCreateOptions = new JpegOptions();


            for (var pageIndex = indexNum; pageIndex < indexNum + 15; pageIndex++)
            {
                var cacheBlobName = CreateCacheFileName(blobName, pageIndex);

                if (await m_BlobProviderCache.ExistsAsync(cacheBlobName, cancelToken).ConfigureAwait(false))
                {
                    blobsNamesInCache.Add(m_BlobProviderCache.GenerateSharedAccessReadPermission(cacheBlobName, 30));
                    continue;
                }
                try
                {

                    var activeTiff = await tiff.Value.ConfigureAwait(false);
                    activeTiff.ActiveFrame = activeTiff.Frames[pageIndex];// tiffFrame;
                    //Load Pixels of TiffFrame into an array of Colors
                    var pixels = activeTiff.LoadPixels(activeTiff.Bounds);

                    //Set the Source of bmpCreateOptions as FileCreateSource by specifying the location where output will be saved
                    using (var ms = new MemoryStream())
                    {
                        jpgCreateOptions.Source = new StreamSource(ms);
                        using (var jpgImage =
                  (JpegImage)Image.Create(jpgCreateOptions, activeTiff.Width, activeTiff.Height))
                        {
                            //Save the bmpImage with pixels from TiffFrame
                            jpgImage.SavePixels(activeTiff.Bounds, pixels);
                            jpgImage.Save();
                        }
                        var gzipSr = await Compress.CompressToGzipAsync(ms, cancelToken).ConfigureAwait(false);
                        parallelTask.Add(m_BlobProviderCache.UploadByteArrayAsync(cacheBlobName, gzipSr, "image/jpg", true, 30));
                        blobsNamesInCache.Add(m_BlobProviderCache.GenerateSharedAccessReadPermission(cacheBlobName, 30));
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }

            }
            await Task.WhenAll(parallelTask).ConfigureAwait(false);
            if (tiff.IsValueCreated)
            {
                tiff.Value.Dispose();
                blobStr.Dispose();
            }
            return new PreviewResult { Content = blobsNamesInCache, ViewName = "Image" };
        }

        protected static string CreateCacheFileName(string blobName, int index)
        {
            return $"{Path.GetFileNameWithoutExtension(blobName)}V4_{index}_{Path.GetExtension(blobName)}.jpg";
        }


        public static readonly string[] TiffExtensions = { ".tiff", ".tif" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.StorageContainerUrl))
            {
                return TiffExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;

        }

        public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                if (await m_BlobProviderPreview.ExistsAsync(blobName + ".jpg", cancelToken).ConfigureAwait(false))
                {
                    return null;
                }


                using (var stream = await BlobProvider.DownloadFileAsync(blobUri, cancelToken).ConfigureAwait(false))
                {
                    using (var ms = new MemoryStream())
                    {
                        var settings2 = new ResizeSettings
                        {
                            Format = "jpg"
                        };
                        ImageBuilder.Current.Build(stream, ms, settings2, false);

                        await m_BlobProviderPreview.UploadStreamAsync(blobName + ".jpg", ms, "image/jpeg", cancelToken).ConfigureAwait(false);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex);
            }
            return null;
        }


        public override Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }

    }
}
