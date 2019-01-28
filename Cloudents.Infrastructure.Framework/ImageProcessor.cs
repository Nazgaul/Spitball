using ImageResizer;
using ImageResizer.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImageResizer.Plugins.Basic;

namespace Cloudents.Infrastructure.Framework
{
    public class ImageProcessor : IPreviewProvider2, IBlurProcessor, IDisposable
    {
        public static readonly string[] Extensions = { ".jpg", ".gif", ".png", ".jpeg", ".bmp" };

        public ImageProcessor()
        {
            new BlurFilter().Install(
                Config.Current);

            Config.Current.Plugins.LoadPlugins();
            Config.Current.Plugins.Get<SizeLimiting>().Uninstall(Config.Current);
        }

        private Stream _sr;
        public void Init(Stream stream)
        {
            _sr = stream;
        }

        public (string text, int pagesCount) ExtractMetaContent()
        {

            return (null, 1);
        }


        public async Task ProcessFilesAsync(IEnumerable<int> previewDelta, Func<Stream, string, Task> pagePreviewCallback,
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
                ImageBuilder.Current.Build(_sr, ms, settings2, false);
                await pagePreviewCallback(ms, "0.jpg");
            }
        }

        public Task ProcessBlurPreviewAsync(Stream stream, bool firstPage,
            Func<Stream, Task> pagePreviewCallback,
            CancellationToken token)
        {

            var ms = new MemoryStream();
            var settings2 = new ResizeSettings
            {
                Format = "jpg",
                Quality = 90,
                ["r.blur"] = "6",
                ["r.blurStart"] = firstPage.ToString()
            };

            ImageBuilder.Current.Build(stream, ms, settings2, false);

            return pagePreviewCallback(ms).ContinueWith(_ => ms.Dispose(), token);
        }


        public void Dispose()
        {
            _sr?.Dispose();
        }
    }



}
