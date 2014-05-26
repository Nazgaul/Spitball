using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class UrlTitleBringer
    {
        public async Task<string> BringTitle(string url)
        {

            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
            {

                var uri = new UriBuilder(url).Uri;
                using (var response = await client.GetAsync(uri))
                {
                    var html = await response.Content.ReadAsStringAsync();
                    var regExCharset = Regex.Match(html, "<meta.*?charset=([^\"']+)").Groups[1].Value;


                    if (string.IsNullOrEmpty(regExCharset))
                    {
                        return ExtractTitle(html);
                    }
                    html = Encoding.GetEncoding(regExCharset).GetString(await response.Content.ReadAsByteArrayAsync());

                    //var response = await client.GetByteArrayAsync(uri);
                    //var html = Encoding.Unicode.GetString(response, 0, response.Length - 1);
                    //return responseString;

                    return ExtractTitle(html);
                }
            }
        }

        private static string ExtractTitle(string html)
        {
            string title = Regex.Match(html, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
            return title;
        }
    }
}