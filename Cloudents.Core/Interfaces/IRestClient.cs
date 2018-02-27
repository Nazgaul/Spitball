using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface IRestClient
    {
        [ItemCanBeNull]
        Task<string> GetAsync(Uri url, NameValueCollection queryString, CancellationToken token);
        
        [ItemCanBeNull]
        Task<string> GetAsync(Uri url, NameValueCollection queryString,
            IEnumerable<KeyValuePair<string, string>> headers,
            CancellationToken token);

        Task<(Stream stream, EntityTagHeaderValue etagHeader)> DownloadStreamAsync(Uri url,
            CancellationToken token);

        Task<(Stream stream, EntityTagHeaderValue etagHeader)> DownloadStreamAsync(Uri url,
            AuthenticationHeaderValue auth, CancellationToken token);

        Task<Uri> UrlRedirectAsync(Uri url);

        [ItemCanBeNull]
        Task<T> GetAsync<T>(Uri url, NameValueCollection queryString, CancellationToken token);
        [ItemCanBeNull]
        Task<T> GetAsync<T>(Uri url, NameValueCollection queryString,
            IEnumerable<KeyValuePair<string, string>> headers,
            CancellationToken token);
    }
}