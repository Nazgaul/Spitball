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
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure
{
    public class RestClient : IRestClient
    {

        public async Task<JObject> GetJsonAsync(Uri url, NameValueCollection queryString, CancellationToken token)
        {
            var str = await GetAsync(url, queryString, token).ConfigureAwait(false);
            return JObject.Parse(str);
        }

        public Task<string> GetAsync(Uri url, NameValueCollection queryString, CancellationToken token)
        {
            return GetAsync(url, queryString, null, token);
        }

        public async Task<string> GetAsync(Uri url, NameValueCollection queryString, IEnumerable<KeyValuePair<string, string>> headers,
            CancellationToken token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                foreach (var header in headers ?? Enumerable.Empty<KeyValuePair<string, string>>())
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = new UriBuilder(url);

                uri.AddQuery(queryString);

                var response = await client.GetAsync(uri.Uri, token).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    return null;
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        public async Task<(Stream, EntityTagHeaderValue)> DownloadStreamAsync(Uri url, HttpClientHandler handler, CancellationToken token)
        {
            //var locationToSave = _localStorage.CombineDirectoryWithFileName(fileName);
            //if (File.Exists(locationToSave) && !@override)
            //{
            //    return locationToSave;
            //}


            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.IfMatch.Add(EntityTagHeaderValue.Parse("\"a53dfa946845e083d0e642de91ec3c05\""));
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",)
                var result = await client.GetAsync(url,
                    HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
                result.EnsureSuccessStatusCode();
                return (await result.Content.ReadAsStreamAsync(), result.Headers.ETag);
                //using (var stream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false))
                //{
                //    return await _localStorage.SaveFileToStorageAsync(stream, fileName)
                //        .ConfigureAwait(false);
                //}
            }

        }


        //public Task<Stream> DownloadStreamAsync(Uri url, CancellationToken token)
        //{
        //    var defaultClientHeader = new HttpClientHandler();
        //    return DownloadStreamAsync(url,  defaultClientHeader, token);
        //}
    }
}
