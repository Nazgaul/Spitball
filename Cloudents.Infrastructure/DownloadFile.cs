using System;
using System.IO;
using System.Net.Http;
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

        public async Task<(string path, DateTime lastWriteTime)> DownloadFileAsync(Uri url, string fileName, HttpClientHandler handler, CancellationToken token)
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

            var result = await _client.DownloadStreamAsync(url, handler, token).ConfigureAwait(false);

            try
            {
                if (result.Item2.Tag == eTag)
                {
                    return (locationToSave, lastWriteTime);
                }

                var tempFile = await _localStorage.SaveFileToStorageAsync(result.Item1, tempFileName);
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
                result.Item1.Dispose();
            }
        }

        private static string CalculateMd5(string locationToSave)
        {
            string eTag;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(locationToSave))
                {
                    var hash = md5.ComputeHash(stream);
                    eTag = $"\"{BitConverter.ToString(hash).Replace("-", string.Empty).ToLower()}\"";
                }
            }

            return eTag;
        }

        public async Task<string> DownloadFileAsync(Uri url, string fileName, bool @override, CancellationToken token)
        {
            using (var defaultClientHeader = new HttpClientHandler())
            {
                var p = await DownloadFileAsync(url, fileName, defaultClientHeader, token).ConfigureAwait(false);
                return p.path;
            }
        }
    }
}
