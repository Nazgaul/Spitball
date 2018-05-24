using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Attributes;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
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
            var stream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false);
            if (result.Content.Headers.ContentType.MediaType.Contains("gzip", StringComparison.OrdinalIgnoreCase))
            {
                stream = await Compress.DecompressFromGzipAsync(stream).ConfigureAwait(false);
            }

            return (stream, result.Headers.ETag);
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
                var serializer = new JsonSerializer
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                };
                return serializer.Deserialize<T>(reader);
            }
        }

        public  Task<bool> PostAsync(Uri url, HttpContent body, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token)
        {
            return TransferHttpContentAsync(HttpMethod.Post, url, body, headers, token);
        }

        [Log]
        public Task<bool> PostJsonAsync<T>(Uri url, T obj, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token)
        {
            return TransferJsonBodyAsync(HttpMethod.Post, url, obj, headers, token);
        }

        [Log]
        public Task<bool> PutJsonAsync<T>(Uri url, T obj, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token)
        {
            return TransferJsonBodyAsync(HttpMethod.Put, url, obj, headers, token);
        }

        private Task<bool> TransferJsonBodyAsync<T>(HttpMethod method, Uri url, T obj, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token)
        {
            var jsonInString = JsonConvert.SerializeObject(obj);
            using (var stringContent = new StringContent(jsonInString, Encoding.UTF8, "application/json"))
            {
                return TransferHttpContentAsync(method, url, stringContent, headers, token);

            }
        }

        private async Task<bool> TransferHttpContentAsync(HttpMethod method, Uri url, HttpContent obj, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token)
        {
            //var jsonInString = JsonConvert.SerializeObject(obj);
            //using (var stringContent = new StringContent(jsonInString, Encoding.UTF8, "application/json"))
            //{
            using (var message = new HttpRequestMessage(method, url)
            {
                Content = obj
            })
            {
                var p = await _client.SendAsync(message, token).ConfigureAwait(false);
                return p.IsSuccessStatusCode;
            }
            //}
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
