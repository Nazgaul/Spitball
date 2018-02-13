using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure
{
    public class RestClient : IRestClient, IDisposable
    {
        private readonly HttpClient _client;

        public RestClient()
        {
            _client = new HttpClient();
        }

        //public async Task<JObject> GetJsonAsync(Uri url, NameValueCollection queryString, CancellationToken token)
        //{
        //    var str = await GetAsync(url, queryString, token).ConfigureAwait(false);
        //    return JObject.Parse(str);
        //}

        public Task<string> GetAsync(Uri url, NameValueCollection queryString, CancellationToken token)
        {
            return GetAsync(url, queryString, null, token);
        }

        public async Task<string> GetAsync(Uri url, NameValueCollection queryString, IEnumerable<KeyValuePair<string, string>> headers,
            CancellationToken token)
        {
            _client.DefaultRequestHeaders.Clear();
            foreach (var header in headers ?? Enumerable.Empty<KeyValuePair<string, string>>())
            {
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var uri = new UriBuilder(url);

            uri.AddQuery(queryString);

            var response = await _client.GetAsync(uri.Uri, token).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public Task<(Stream stream, EntityTagHeaderValue etagHeader)> DownloadStreamAsync(Uri url,
             CancellationToken token)
        {
            return DownloadStreamAsync(url, default, token);
        }

        public async Task<(Stream stream, EntityTagHeaderValue etagHeader)> DownloadStreamAsync(Uri url, AuthenticationHeaderValue auth, CancellationToken token)
        {
            _client.DefaultRequestHeaders.Clear();
            if (auth != null)
            {
                _client.DefaultRequestHeaders.Authorization = auth;
            }

            var result = await _client.GetAsync(url,
                    HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
            result.EnsureSuccessStatusCode();
            return (await result.Content.ReadAsStreamAsync().ConfigureAwait(false), result.Headers.ETag);
        }

        public async Task<Uri> UrlRedirectAsync(Uri url)
        {
            var response = await _client.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return response.RequestMessage.RequestUri;
        }

        public Task<T> GetAsync<T>(Uri url, NameValueCollection queryString, CancellationToken token)
        {
            return GetAsync<T>(url, queryString, null, token);
        }

        public async Task<T> GetAsync<T>(Uri url, NameValueCollection queryString, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token)
        {
            _client.DefaultRequestHeaders.Clear();
            foreach (var header in headers ?? Enumerable.Empty<KeyValuePair<string, string>>())
            {
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var uri = new UriBuilder(url);

            uri.AddQuery(queryString);

            var response = await _client.GetAsync(uri.Uri, token).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                return default;
            using (var s = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            using (var sr = new StreamReader(s))
            using (var reader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
