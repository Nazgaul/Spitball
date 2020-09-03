using Aspose.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Framework
{
    public sealed class ImageProcessor : IPreviewProvider, IDisposable
    {

        public ImageProcessor()
        {
            using (var sr = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cloudents.Infrastructure.Framework.Aspose.Total.lic"))
            {
                var license = new License();
                license.SetLicense(sr);
            }
        }

        private Stream _sr;
        public void Init(Func<Stream> stream)
        {
            _sr = stream();
        }

        public void Init(Func<string> stream)
        {
            _sr = File.Open(stream(), FileMode.Open);
        }

        public (string text, int pagesCount) ExtractMetaContent()
        {

            return (null, 1);
        }

        public async Task ProcessFilesAsync(IEnumerable<int> previewDelta, Func<Stream, string, Task> pagePreviewCallback,
            CancellationToken token)
        {
            using var image = Image.Load(_sr);
            if (image.Width > 1920)
            {
                using var ms = new MemoryStream();
                int originalWidth = image.Width;
                int originalHeight = image.Height;

                // To preserve the aspect ratio
                float ratioX = (float)1920 / (float)originalWidth;

                // New width and height based on aspect ratio
                int newWidth = (int)(originalWidth * ratioX);
                int newHeight = (int)(originalHeight * ratioX);
                image.Resize(newWidth, newHeight);
                image.Save(ms, new Aspose.Imaging.ImageOptions.JpegOptions()
                {
                    Quality = 80
                });
                await pagePreviewCallback(ms, "0.jpg");

            }
            await pagePreviewCallback(_sr, "0.jpg");
        }

        public void Dispose()
        {
            _sr?.Dispose();
        }

    }



}
