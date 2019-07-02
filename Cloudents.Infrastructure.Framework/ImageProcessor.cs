using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;

namespace Cloudents.Infrastructure.Framework
{
    public class ImageProcessor : IPreviewProvider2, IDisposable
    {
        public static readonly string[] Extensions = FormatDocumentExtensions.Image;



        private Stream _sr;
        public void Init(Func<Stream> stream)
        {
            _sr = stream();
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
                var image = Image.Load<Rgba32>(_sr);
                image.SaveAsJpeg(ms);
                await pagePreviewCallback(ms, "0.jpg");
            }
        }

        public void Dispose()
        {
            _sr?.Dispose();
        }

    }



}
