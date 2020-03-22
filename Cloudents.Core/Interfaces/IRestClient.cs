using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IRestClient
    {
       // Task<string?> GetAsync(Uri url, NameValueCollection queryString, CancellationToken token);

        //[ItemCanBeNull]
        //Task<string> GetAsync(Uri url, NameValueCollection queryString,
        //    IEnumerable<KeyValuePair<string, string>> headers,
        //    CancellationToken token);

        Task<(Stream stream, EntityTagHeaderValue etagHeader)> DownloadStreamAsync(Uri url,
            CancellationToken token);

        Task<(Stream stream, EntityTagHeaderValue etagHeader)> DownloadStreamAsync(Uri url,
            AuthenticationHeaderValue auth, CancellationToken token);

        //Task<Uri> UrlRedirectAsync(Uri url);

        Task<T> GetAsync<T>(Uri url, NameValueCollection? queryString, CancellationToken token);

        //Task<T> GetAsync<T>(Uri url, NameValueCollection queryString,
        //    IEnumerable<KeyValuePair<string, string>> headers,
        //    CancellationToken token);

        //Task<bool> PostAsync(Uri url, HttpContent body, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token);

        //Task<bool> PostJsonAsync<T>(Uri url, T obj, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token);
        //Task<TU> PostJsonAsync<T, TU>(Uri url, T obj, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token);

        //Task<TU> PostJsonAsync<TU>(Uri url, string body, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token);


        //Task<HttpResponseMessage> PostAsync(Uri url, HttpContent body,
        //    IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token);

        //Task<bool> PutJsonAsync<T>(Uri url, T obj, IEnumerable<KeyValuePair<string, string>> headers,
        //CancellationToken token);
    }
}