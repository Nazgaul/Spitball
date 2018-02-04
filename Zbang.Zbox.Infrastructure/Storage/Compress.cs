using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public static class Compress
    {
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
