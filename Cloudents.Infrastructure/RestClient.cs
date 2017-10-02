using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure
{
    public class RestClient : IRestClient
    {
        public async Task<JObject> GetAsync(Uri url, NameValueCollection queryString, CancellationToken token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = new UriBuilder(url);

                uri.AddQuery(queryString);

                var response = await client.GetAsync(uri.Uri, token).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode) return null;
                var str = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JObject.Parse(str);
            }
        }
    }

    public interface IRestClient
    {
        Task<JObject> GetAsync(Uri url, NameValueCollection queryString, CancellationToken token);
    }
}
