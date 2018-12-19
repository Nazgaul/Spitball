using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Application.Interfaces
{
    public interface IDownloadFile
    {
        Task<(string path, DateTime lastWriteTime)> DownloadFileAsync(Uri url, string fileName,
            AuthenticationHeaderValue auth, CancellationToken token);
    }
}