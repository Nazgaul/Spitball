using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Search
{
    public class TitleSearch : ITitleSearch
    {
        public async Task<string> SearchAsync(string term, CancellationToken token)
        {
            using (var client = new HttpClient())
            {
                var uri = new UriBuilder("https://api.wolframalpha.com/v2/result");
                var nvc = new NameValueCollection
                {
                    ["input"] = term,
                    ["appid"] = "HQRVE6-VPJVLT32RK"
                };
                uri.AddQuery(nvc);

                var response = await client.GetAsync(uri.Uri, token).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode) return null;

                var str = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (str.Contains(term, StringComparison.InvariantCultureIgnoreCase) ||
                    term.Contains(str, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                return str;
            }
        }
    }
}
