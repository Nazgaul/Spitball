using System.Threading;
using System;
using System.IO;
using System.Threading.Tasks;
using ImageResizer;

namespace Cloudents.Infrastructure.Framework
{
    public class ImageProcessor : IPreviewProvider2
    {
        public static readonly string[] ImageExtensions = { ".jpg", ".gif", ".png", ".jpeg", ".bmp" };

        public async Task ProcessFilesAsync(Stream stream, 
            Func<Stream, string, Task> pagePreviewCallback,
            Func<string, Task> textCallback, 
            Func<int, Task> pageCountCallback,
            CancellationToken token)
        {
            await pageCountCallback(1);
            using (var ms = new MemoryStream())
            {
                var settings2 = new ResizeSettings
                {
                    Format = "jpg",
                    Width = 1024,
                    Height = 768
                };
                ImageBuilder.Current.Build(stream, ms, settings2, false);


                await pagePreviewCallback(ms, "0.jpg");
            }
        }


    }
}
