using ImageResizer;
using ImageResizer.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Framework
{
    public class ImageProcessor : IPreviewProvider2, IBlurProcessor
    {
        public static readonly string[] Extensions = { ".jpg", ".gif", ".png", ".jpeg", ".bmp" };

        public ImageProcessor()
        {
            new BlurFilter().Install(
                Config.Current);
        }
        Stream sr;
        public void Init(Stream stream)
        {
            sr = stream;
        }

        public (string text, int pagesCount) ExtractMetaContent()
        {

            return (null, 1);
        }
        public int ExtractPagesCount()
        {
            return 1;
        }

        public async Task ProcessFilesAsync(List<int> previewDelta, Func<Stream, string, Task> pagePreviewCallback,
            CancellationToken token)
        {
            using (var ms = new MemoryStream())
            {
                var settings2 = new ResizeSettings
                {
                    Format = "jpg",
                    Width = 1024,
                    Height = 768,
                    Quality = 90,
                };
                ImageBuilder.Current.Build(sr, ms, settings2, false);
                await pagePreviewCallback(ms, $"0.jpg");
            }
        }

        public async Task ProcessBlurPreviewAsync(Stream stream, bool firstPage,
            Func<Stream, Task> pagePreviewCallback,
            CancellationToken token)
        {
            
            using (var ms = new MemoryStream())
            {
                var settings2 = new ResizeSettings
                {
                    Format = "jpg",
                    Quality = 90,
                    ["r.blur"] = "10",
                    ["r.blurStart"] = firstPage.ToString()
                };

                ImageBuilder.Current.Build(stream, ms, settings2, false);

                await pagePreviewCallback(ms);
            }
        }


    }



}
