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

        public async Task<string> DownloadFileAsync(Uri url, string fileName, bool @override, HttpClientHandler handler, CancellationToken token)
        {
            var locationToSave = _localStorage.CombineDirectoryWithFileName(fileName);
            string eTag = null;
            if (File.Exists(locationToSave))
            {
                if (!@override)
                {
                    return locationToSave;
                }
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(locationToSave))
                    {
                        var hash = md5.ComputeHash(stream);
                        eTag = $"\"{BitConverter.ToString(hash).Replace("-", string.Empty).ToLower()}\"";
                    }
                }
            }

            var result = await _client.DownloadStreamAsync(url, handler, token);

            try
            {
                if (result.Item2.Tag == eTag)
                {
                    return locationToSave;
                }
                return await _localStorage.SaveFileToStorageAsync(result.Item1, fileName)
                    .ConfigureAwait(false);
            }
            finally
            {
                result.Item1.Dispose();
            }

        }

        public async Task<string> DownloadFileAsync(Uri url, string fileName, bool @override, CancellationToken token)
        {
            using (var defaultClientHeader = new HttpClientHandler())
            {
                return await DownloadFileAsync(url, fileName, @override, defaultClientHeader, token).ConfigureAwait(false);
            }

        }
    }
}
