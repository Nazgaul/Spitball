using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
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


    public static class BlobExtensions
    {
        /// <summary>
        /// Used for processing image since SixLabors doesn't handle good the blob.OpenReadAsync
        /// </summary>
        /// <param name="blob"></param>
        /// <returns></returns>
        public static async Task<Stream> OpenStreamAsync(this CloudBlockBlob blob)
        {
            var sr = new MemoryStream();
            await blob.DownloadToStreamAsync(sr);
            sr.Seek(0, SeekOrigin.Begin);
            return sr;
        }
    }
}