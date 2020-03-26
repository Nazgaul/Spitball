using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Storage
{
    public static class Compress
    {
        //public static byte[] CompressToGzip(byte[] stream)
        //{
        //    if (stream == null) throw new ArgumentNullException(nameof(stream));
        //    using (var ms = new MemoryStream())
        //    {
        //        using (var gz = new GZipStream(ms, CompressionMode.Compress))
        //        {
        //            gz.Write(stream, 0, stream.Length);
        //        }
        //        return ms.ToArray();
        //    }
        //}

        //public static byte[] CompressToGzip(Stream stream)
        //{
        //    if (stream == null) throw new ArgumentNullException(nameof(stream));
        //    stream.Seek(0, SeekOrigin.Begin);
        //    using (var ms = new MemoryStream())
        //    {
        //        using (var gz = new GZipStream(ms, CompressionMode.Compress))
        //        {
        //            stream.CopyTo(gz);
        //        }

        //        return ms.ToArray();
        //    }
        //}

        public static async Task<Stream> DecompressFromGzipAsync(Stream stream)
        {
            var output = new MemoryStream();
            await using (Stream cs = new GZipStream(stream, CompressionMode.Decompress))
            await cs.CopyToAsync(output);

            return output;
        }

        //public static async Task<Stream> CompressToGzipAsync(Stream stream, CancellationToken token = default(CancellationToken))
        //{
        //    stream.Seek(0, SeekOrigin.Begin);
        //    var ms = new MemoryStream();
        //    using (var gz = new GZipStream(ms, CompressionLevel.Optimal))
        //    {
        //        await stream.CopyToAsync(gz, 81920, token);
        //    }
        //    return ms;
        //}
    }
}
