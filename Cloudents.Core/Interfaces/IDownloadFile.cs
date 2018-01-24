﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IDownloadFile
    {
        Task<string> DownloadFileAsync(Uri url, string fileName, bool @override, HttpClientHandler handler, CancellationToken token);
        Task<string> DownloadFileAsync(Uri url, string fileName, bool @override, CancellationToken token);
    }
}