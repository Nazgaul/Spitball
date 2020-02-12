using Cloudents.Core.Attributes;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

namespace Cloudents.Infrastructure
{
    [UsedImplicitly]
    public sealed class RestClient : IRestClient
    {
        private readonly HttpClient _client;


        public RestClient()
        {
            _client = new HttpClient();
        }

        public RestClient(HttpClient client)
        {
            _client = client;
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

            var response = await _client.GetAsync(uri.Uri, token);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadAsStringAsync();
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
                    HttpCompletionOption.ResponseHeadersRead, token);
            result.EnsureSuccessStatusCode();
            var stream = await result.Content.ReadAsStreamAsync();
            if (result.Content.Headers.ContentType.MediaType.Contains("gzip", StringComparison.OrdinalIgnoreCase))
            {
                stream = await Compress.DecompressFromGzipAsync(stream);
            }

            return (stream, result.Headers.ETag);
        }

        public async Task<Uri> UrlRedirectAsync(Uri url)
        {
            var response = await _client.GetAsync(url);
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

            var response = await _client.GetAsync(uri.Uri, token);
            response.EnsureSuccessStatusCode();
            using (var s = await response.Content.ReadAsStreamAsync())
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

        //public async Task<bool> PostAsync(Uri url, HttpContent body, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token)
        //{
        //    var t = await TransferHttpContentAsync(HttpMethod.Post, url, body, headers, token);
        //    return t.IsSuccessStatusCode;
        //}

        public async Task<HttpResponseMessage> PostAsync(Uri url, HttpContent body,
            IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token)
        {
            return await TransferHttpContentAsync(HttpMethod.Post, url, body, headers, token);
        }

        [Log]
        public async Task<bool> PostJsonAsync<T>(Uri url, T obj, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token)
        {
            var t = await TransferJsonBodyAsync(HttpMethod.Post, url, obj, headers, token);
            return t.IsSuccessStatusCode;
        }

        public async Task<Tu> PostJsonAsync<T, Tu>(Uri url, T obj, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token)
        {
            var response = await TransferJsonBodyAsync(HttpMethod.Post, url, obj, headers, token);
            if (!response.IsSuccessStatusCode)
            {

                var content = await response.Content.ReadAsStringAsync();

                var v = new HttpRequestException($"failed to invoke url: {url}")
                {
                    Data =
                    {
                        ["content"] = content,
                        ["obj"] = JsonConvert.SerializeObject(obj),
                        ["headers"] = JsonConvert.SerializeObject(headers)
                    }

                };
                throw v;
            }

            //response.EnsureSuccessStatusCode();
            using (var s = await response.Content.ReadAsStreamAsync())
            using (var sr = new StreamReader(s))
            using (var reader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                };
                return serializer.Deserialize<Tu>(reader);
            }
        }



        //[Log]
        //public async Task<bool> PutJsonAsync<T>(Uri url, T obj, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token)
        //{
        //    var t = await TransferJsonBodyAsync(HttpMethod.Put, url, obj, headers, token);
        //    return t.IsSuccessStatusCode;
        //}

        private async Task<HttpResponseMessage> TransferJsonBodyAsync<T>(HttpMethod method, Uri url, T obj, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token)
        {
            var jsonInString = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }

            });
            using (var stringContent = new StringContent(jsonInString, Encoding.UTF8,
                "application/json"))
            {
                return await TransferHttpContentAsync(method, url, stringContent, headers, token);
            }
        }

        private async Task<HttpResponseMessage> TransferHttpContentAsync(HttpMethod method, Uri url, HttpContent obj, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken token)
        {
            using (var message = new HttpRequestMessage(method, url)
            {
                Content = obj
            })
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        message.Headers.Add(header.Key, header.Value);
                    }
                }

                var p = await _client.SendAsync(message, token);
                //if (!p.IsSuccessStatusCode)
                //{
                //    var content = await p.Content.ReadAsStringAsync();
                //    _logger.Warning(content);
                //}
                return p;
            }
        }

        //public void Dispose()
        //{
        //    _client?.Dispose();
        //}
    }
}
