﻿using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public static class Compress
    {
        public static byte[] CompressToGzip(byte[] stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            using (var ms = new MemoryStream())
            {
                using (var gz = new GZipStream(ms, CompressionMode.Compress))
                {
                    gz.Write(stream, 0, stream.Length);
                }
                return ms.ToArray();
            }
        }

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

        public static byte[] DecompressFromGzip(byte[] byteArray)
        {
            using (var input = new MemoryStream(byteArray))
            using (var output = new MemoryStream())
            {
                using (Stream cs = new GZipStream(input, CompressionMode.Decompress))
                {
                    cs.CopyTo(output);
                }

                return output.ToArray();
            }
        }

        public static async Task<byte[]> CompressToGzipAsync(Stream stream, CancellationToken token = default(CancellationToken))
        {
            stream.Seek(0, SeekOrigin.Begin);
            var ms = new MemoryStream();
            using (var gz = new GZipStream(ms, CompressionMode.Compress))
            {
                await stream.CopyToAsync(gz, 81920, token).ConfigureAwait(false);
            }
            return ms.ToArray();
        }
    }
}
