﻿using System.Threading;
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
        public TiffProcessor(IBlobProvider blobProvider, IBlobProvider2<IStorageContainerName> blobProviderPreview)
            : base(blobProvider, blobProviderPreview)
        {

        }
        private static void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }
        public override async Task<PreviewResult> ConvertFileToWebSitePreviewAsync(Uri blobUri, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            // var indexOfPageGenerate = CalculateTillWhenToDrawPictures(indexNum);
            Stream blobStr = null;
            var tiff = new AsyncLazy<TiffImage>(async () =>
           {
               SetLicense();
               blobStr = await BlobProvider.DownloadFileAsync(blobUri, cancelToken);

               var tiffImage = (TiffImage)Image.Load(blobStr);
               return tiffImage;


           });
            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task<string>>();
            var jpgCreateOptions = new JpegOptions();


            for (var pageIndex = indexNum; pageIndex < indexNum + 15; pageIndex++)
            {
                var cacheblobName = CreateCacheFileName(blobName, pageIndex);
                var cacheBlobNameWithSharedAccessSignature = BlobProvider.GenerateSharedAccressReadPermissionInCache(cacheblobName, 20);
                if (!string.IsNullOrEmpty(cacheBlobNameWithSharedAccessSignature))
                {
                    blobsNamesInCache.Add(cacheBlobNameWithSharedAccessSignature);
                    continue;
                }
                try
                {

                    var activeTiff = await tiff.Value;
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
                        var compressor = new Compress();
                        var gzipSr = compressor.CompressToGzip(ms);
                        parallelTask.Add(BlobProvider.UploadFileToCacheAsync(cacheblobName, gzipSr, "image/jpg", true));
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }

            }
            await Task.WhenAll(parallelTask);
            blobsNamesInCache.AddRange(parallelTask.Select(s => s.Result));
            if (tiff.IsValueCreated)
            {
                tiff.Value.Dispose();
                blobStr.Dispose();
            }
            return new PreviewResult { Content = blobsNamesInCache, ViewName = "Image" };
        }

        protected string CreateCacheFileName(string blobName, int index)
        {
            return string.Format("{0}V4_{2}_{1}.jpg", Path.GetFileNameWithoutExtension(blobName), Path.GetExtension(blobName), index);
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
                try
                {
                    var blobName = GetBlobNameFromUri(blobUri);

                    using (var stream = await BlobProvider.DownloadFileAsync(blobUri, cancelToken))
                    {
                        using (var ms = new MemoryStream())
                        {
                            var settings2 = new ResizeSettings
                            {
                                Format = "jpg"
                            };
                            ImageBuilder.Current.Build(stream, ms, settings2, false);

                            await BlobProviderPreview.UploadStreamAsync(blobName + ".jpg", ms, "image/jpeg", cancelToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("PreProcessFile tiff", ex);
                }
                return null;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile tiff", ex);
            }
            return null;
        }

        //public override string GetDefaultThumbnailPicture()
        //{
        //    return DefaultPicture.ImageFileTypePicture;
        //}

        public override Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }

        //public override async Task GenerateImagePreviewAsync(Uri blobUri, CancellationToken cancelToken)
        //{

        //}
    }
}
