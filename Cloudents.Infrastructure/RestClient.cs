using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
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

        public async Task<string> GetAsync(Uri url, NameValueCollection queryString, IEnumerable<KeyValuePair<string,string>> headers,
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
    }
}
