using System.Threading;
using Aspose.Imaging;
using Aspose.Imaging.FileFormats.Jpeg;
using Aspose.Imaging.FileFormats.Tiff;
using Aspose.Imaging.ImageOptions;
using Aspose.Imaging.Sources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Storage;
using Image = Aspose.Imaging.Image;

namespace Cloudents.Infrastructure.Framework
{
    public class TiffProcessor : Processor, IPreviewProvider
    {
        public TiffProcessor(
                Uri blobUri,
                IBlobProvider blobProvider,
                IBlobProvider<CacheContainer> blobProviderCache)
            //: base(blobProvider, blobProviderPreview, blobProviderCache)
            : base(blobProvider, blobProviderCache, blobUri)
        {
            SetLicense();
        }

        private static void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }

        public async Task<IEnumerable<string>> ConvertFileToWebsitePreviewAsync(int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = BlobProvider.GetBlobNameFromUri(BlobUri);
            Stream blobStr = null;
            var tiff = new AsyncLazy<TiffImage>(async () =>
           {
               //SetLicense();
               blobStr = await BlobProvider.DownloadFileAsync(BlobUri, cancelToken).ConfigureAwait(false);

               return (TiffImage)Image.Load(blobStr);
           });
            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task>();
            var jpgCreateOptions = new JpegOptions();

            for (var pageIndex = indexNum; pageIndex < indexNum + 15; pageIndex++)
            {
                var cacheBlobName = CreateCacheFileName(blobName, pageIndex);

                if (await BlobProviderCache.ExistsAsync(cacheBlobName, cancelToken).ConfigureAwait(false))
                {
                    blobsNamesInCache.Add(BlobProviderCache.GenerateSharedAccessReadPermission(cacheBlobName, 30));
                    continue;
                }
                try
                {
                    var activeTiff = await tiff.Instance.Value.ConfigureAwait(false);
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
                        parallelTask.Add(BlobProviderCache.UploadStreamAsync(cacheBlobName, gzipSr, "image/jpg", true, 30, cancelToken));
                        blobsNamesInCache.Add(BlobProviderCache.GenerateSharedAccessReadPermission(cacheBlobName, 30));
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            }
            await Task.WhenAll(parallelTask).ConfigureAwait(false);
            if (tiff.Instance.IsValueCreated)
            {
                tiff.Instance.Value.Dispose();
                blobStr.Dispose();
            }
            return blobsNamesInCache;
        }

        protected static string CreateCacheFileName(string blobName, int index)
        {
            return $"{Path.GetFileNameWithoutExtension(blobName)}V4_{index}_{Path.GetExtension(blobName)}.jpg";
        }

        public static readonly string[] TiffExtensions = { ".tiff", ".tif" };

        //public override bool CanProcessFile(Uri blobName)
        //{
        //    if (blobName.AbsoluteUri.StartsWith(BlobProvider.StorageContainerUrl))
        //    {
        //        return TiffExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        //    }
        //    return false;
        //}

        //public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        //{
        //    try
        //    {
        //        var blobName = GetBlobNameFromUri(blobUri);
        //        if (await m_BlobProviderPreview.ExistsAsync(blobName + ".jpg", cancelToken).ConfigureAwait(false))
        //        {
        //            return null;
        //        }

        //        using (var stream = await BlobProvider.DownloadFileAsync(blobUri, cancelToken).ConfigureAwait(false))
        //        {
        //            using (var ms = new MemoryStream())
        //            {
        //                var settings2 = new ResizeSettings
        //                {
        //                    Format = "jpg"
        //                };
        //                ImageBuilder.Current.Build(stream, ms, settings2, false);

        //                await m_BlobProviderPreview.UploadStreamAsync(blobName + ".jpg", ms, "image/jpeg", cancelToken).ConfigureAwait(false);
        //            }
        //        }

        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        m_Logger.Exception(ex);
        //    }
        //    return null;
        //}

        //public override Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        //{
        //    return Task.FromResult<string>(null);
        //}
    }
}
