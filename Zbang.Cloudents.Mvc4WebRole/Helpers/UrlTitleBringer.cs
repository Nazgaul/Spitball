using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class UrlTitleBringer
    {
        public async Task<string> BringTitle(string url)
        {
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
            {
                var uri = new UriBuilder(url).Uri;
                var html = await client.GetStringAsync(uri);

                //var response = await client.GetByteArrayAsync(uri);
                //var html = Encoding.Unicode.GetString(response, 0, response.Length - 1);
                //return responseString;

                string title = Regex.Match(html, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                return title;
            }
        }
    }
}