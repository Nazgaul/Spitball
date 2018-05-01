using System;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure
{
    public class DownloadFile : IDownloadFile
    {
        private readonly ITempStorageProvider _localStorage;
        private readonly IRestClient _client;

        public DownloadFile(ITempStorageProvider tempStorage, IRestClient client)
        {
            _localStorage = tempStorage;
            _client = client;
        }

        public async Task<(string path, DateTime lastWriteTime)> DownloadFileAsync(Uri url, string fileName, AuthenticationHeaderValue auth, CancellationToken token)
        {
            var tempFileName = Path.GetFileNameWithoutExtension(fileName) + ".temp" + Path.GetExtension(fileName);
            var locationToSave = _localStorage.CombineDirectoryWithFileName(fileName);
            string eTag = null;
            var lastWriteTime = DateTime.UtcNow;
            if (File.Exists(locationToSave))
            {
                var p = new FileInfo(locationToSave);
                //if (!@override)
                //{
                //    return locationToSave;
                //}
                eTag = CalculateMd5(locationToSave);
                lastWriteTime = p.LastWriteTimeUtc;
            }

            var (stream, etagHeader) = await _client.DownloadStreamAsync(url, auth, token).ConfigureAwait(false);

            try
            {
                if (etagHeader != null && etagHeader.Tag == eTag) //using ? will do null == null => true
                {
                    return (locationToSave, lastWriteTime);
                }
                var tempFile = await _localStorage.SaveFileToStorageAsync(stream, tempFileName).ConfigureAwait(false);
                var tempFileEtag = CalculateMd5(tempFile);
                if (tempFileEtag == eTag)
                {
                    return (locationToSave, lastWriteTime);
                }

                if (File.Exists(locationToSave))
                {
                    File.Delete(locationToSave);
                }
                File.Move(tempFile, locationToSave);
                return (locationToSave, DateTime.UtcNow);
            }
            finally
            {
                stream.Dispose();
            }
        }

        private static string CalculateMd5(string locationToSave)
        {
            string eTag;
            using (var stream = File.OpenRead(locationToSave))
            {
                eTag = CalculateMd5(stream);
            }

            return eTag;
        }

        private static string CalculateMd5(Stream stream)
        {
            string eTag;
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(stream);
                eTag = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            }

            return eTag;
        }

    }
}
