using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public class Compress
    {
        public byte[] CompressToGzip(byte[] stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            using (var ms = new MemoryStream())
            {
                using (var gz = new GZipStream(ms, CompressionMode.Compress))
                {
                    gz.Write(stream, 0, stream.Length);
                }
                return ms.ToArray();
            }
        }

        public byte[] CompressToGzip(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            stream.Seek(0, SeekOrigin.Begin);
            using (var ms = new MemoryStream())
            {
                using (var gz = new GZipStream(ms, CompressionMode.Compress))
                {
                    stream.CopyTo(gz);
                    //gz.Write(stream, 0, stream.Length);
                    //stream.CopyTo(gz);

                }

                return ms.ToArray();
            }
        }

        public byte[] DecompressFromGzip(byte[] byteArray)
        {
            using (var input = new MemoryStream(byteArray))
            {
                using (var output = new MemoryStream())
                {
                    using (Stream cs = new GZipStream(input, CompressionMode.Decompress))
                    {
                        cs.CopyTo(output);
                    }

                    var result = output.ToArray();
                    return result;
                }
            }
        }

        public async Task<byte[]> CompressToGzipAsync(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var ms = new MemoryStream();
            using (var gz = new GZipStream(ms, CompressionMode.Compress))
            {
                await stream.CopyToAsync(gz);
                //stream.CopyTo(gz);
                //gz.Write(stream, 0, stream.Length);
                //stream.CopyTo(gz);

            }

            return ms.ToArray();
        }
    }
}
