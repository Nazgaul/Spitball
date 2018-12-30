using ImageResizer;
using ImageResizer.Configuration;
using System;
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


        public async Task ProcessFilesAsync(Stream stream,
            Func<Stream, string, Task> pagePreviewCallback,

            Func<string, int, Task> metaCallback,
            CancellationToken token)
        {


            //TODO: we can do it faster


            await metaCallback(null, 1);
            using (var ms = new MemoryStream())
            {
                var settings2 = new ResizeSettings
                {
                    Format = "jpg",
                    Width = 1024,
                    Height = 768,
                    Quality = 90,

                };

                ImageBuilder.Current.Build(stream, ms, settings2, false);


                await pagePreviewCallback(ms, "0.jpg");
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
