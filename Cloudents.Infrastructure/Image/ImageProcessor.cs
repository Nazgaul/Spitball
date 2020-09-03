using System;
using System.IO;
using Cloudents.Core.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Cloudents.Infrastructure.Image
{
    public class ImageProcessor : IImageProcessor
    {
        public Stream ConvertToJpg(Stream read, int quality = 80, int? maxWidth = null)
        {
            var image = SixLabors.ImageSharp.Image.Load(read);
            var memoryStream = new MemoryStream();
            if (maxWidth != null && image.Width > maxWidth)
            {
                // Get the image's original width and height
                int originalWidth = image.Width;
                int originalHeight = image.Height;

                // To preserve the aspect ratio
                float ratioX = (float)maxWidth / (float)originalWidth;
               // float ratioY = (float)maxHeight / (float)originalHeight;
                //float ratio = Math.Min(ratioX, ratioY);

                // New width and height based on aspect ratio
                int newWidth = (int)(originalWidth * ratioX);
                int newHeight = (int)(originalHeight * ratioX);
                image.Mutate(x=>x.Resize(newWidth,newHeight));
            }
            image.SaveAsJpeg(memoryStream, new JpegEncoder()
            {
                Quality = quality
            });
            return memoryStream;
        }
    }
}
