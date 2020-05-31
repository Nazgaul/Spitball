using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure
{
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

        public Task<(Stream stream, EntityTagHeaderValue etagHeader)> DownloadStreamAsync(Uri url,
             CancellationToken token)
        {
            return DownloadStreamAsync(url, default, token);
        }

        private async Task<(Stream stream, EntityTagHeaderValue etagHeader)> DownloadStreamAsync(Uri url,
            AuthenticationHeaderValue? auth, CancellationToken token)
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

       

       

      

    

    

        

      
    }

    public static class HttpClientExtensions
    {
        public static async Task<T> GetAsJsonAsync<T>(this HttpClient client, Uri uri, CancellationToken token)
        {
            var response = await client.GetAsync(uri, token);
            response.EnsureSuccessStatusCode();
            await using var s = await response.Content.ReadAsStreamAsync();
            using var sr = new StreamReader(s);
            using var reader = new JsonTextReader(sr);
            var serializer = new JsonSerializer
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            return serializer.Deserialize<T>(reader);
        }
    }
}
