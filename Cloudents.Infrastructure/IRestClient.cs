using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure
{
    public interface IRestClient
    {
        Task<string> GetAsync(Uri url, NameValueCollection queryString, CancellationToken token);
        Task<JObject> GetJsonAsync(Uri url, NameValueCollection queryString, CancellationToken token);

        Task<string> GetAsync(Uri url, NameValueCollection queryString,
            IEnumerable<KeyValuePair<string, string>> headers,
            CancellationToken token);

        Task<(Stream stream, EntityTagHeaderValue etagHeader)> DownloadStreamAsync(Uri url, HttpClientHandler handler, CancellationToken token);
    }
}