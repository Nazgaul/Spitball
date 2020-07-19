using System.IO;
using Cloudents.Core.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Cloudents.Infrastructure.Image
{
    public class ImageProcessor : IImageProcessor
    {
        public Stream ConvertToJpg(Stream read, int quality = 80)
        {
            var image = SixLabors.ImageSharp.Image.Load(read);
            var memoryStream = new MemoryStream();
            image.SaveAsJpeg(memoryStream, new JpegEncoder()
            {
                Quality = quality
            });
            return memoryStream;
        }
    }
}
