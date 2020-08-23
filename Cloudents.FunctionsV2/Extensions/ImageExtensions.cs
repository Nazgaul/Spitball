using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Cloudents.FunctionsV2.Extensions
{
    public static class ImageExtensions
    {
        public static Stream SaveAsJpeg(this Image image)
        {
            var ms = new MemoryStream();
            image.SaveAsJpeg(ms,new JpegEncoder()
            {
                Quality = 80
            });
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}