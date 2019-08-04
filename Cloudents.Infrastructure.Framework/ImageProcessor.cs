﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Imaging;
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
            using (var ms = new MemoryStream())
            {
                using (var image = Aspose.Imaging.Image.Load(_sr))
                {
                    image.Resize(1024,768);
                    image.Save(ms,new Aspose.Imaging.ImageOptions.JpegOptions()
                    {
                        Quality = 90
                    });
                    await pagePreviewCallback(_sr, "0.jpg");
                }
            }
        }

        public void Dispose()
        {
            _sr?.Dispose();
        }

    }



}
