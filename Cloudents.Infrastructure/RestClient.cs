using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Attributes;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure
{
    [UsedImplicitly]
    public sealed class RestClient : IRestClient, IDisposable
    {
        private readonly HttpClient _client;

        public RestClient()
        {
            _client = new HttpClient();
        }

        [CanBeNull]
        [Log]
        public Task<string> GetAsync(Uri url, NameValueCollection queryString, CancellationToken token)
        {
            return GetAsync(url, queryString, null, token);
        }

        [Log]
        public async Task<string> GetAsync(Uri url, NameValueCollection queryString,
            IEnumerable<KeyValuePair<string, string>> headers,
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

        [Log]
        public Task<T> GetAsync<T>(Uri url, NameValueCollection queryString, CancellationToken token)
        {
            return GetAsync<T>(url, queryString, null, token);
        }

        [Log]
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
            response.EnsureSuccessStatusCode();
            using (var s = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            using (var sr = new StreamReader(s))
            using (var reader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }

        public async Task<bool> PostAsync(Uri url, HttpContent body, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token)
        {
            _client.DefaultRequestHeaders.Clear();
            foreach (var header in headers ?? Enumerable.Empty<KeyValuePair<string, string>>())
            {
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var p = await _client.PostAsync(url, body, token).ConfigureAwait(false);
            return p.IsSuccessStatusCode;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
