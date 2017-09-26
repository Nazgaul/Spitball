using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Newtonsoft.Json.Linq;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Search
{
    public class VideoSearch:IVideoSearch
    {
        public async Task<VideoDto> SearchAsync(string term, CancellationToken token)
        {
            using (var client = new HttpClient())
            {
                var uri = new UriBuilder("https://www.googleapis.com/youtube/v3/search");
                var nvc = new NameValueCollection
                {
                    ["q"]= term,
                    ["part"] = "snippet",
                    ["type"]="video",
                    ["videoDuration"]="short",
                    ["videoEmbeddable"]="true",
                    ["videoSyndicated"]="true",
                    ["videoCategoryId"]= "27",
                    ["maxResults"]="1",
                    ["key"]="AIzaSyCAaZGgVHm0GxY2lY_mWQw3JXGy7KMypZ0"
                };
                uri.AddQuery(nvc);

                var response = await client.GetAsync(uri.Uri, token).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode) return null;

                var str = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var o = JObject.Parse(str);
                var firstVideo = o["items"].Children().FirstOrDefault();

                if (firstVideo == null)
                {
                    return null;
                }
                var result = new VideoDto
                {
                    Url = $"https://www.youtube.com/embed/{firstVideo["id"]["videoId"].Value<string>()}"
                };
                return result;



            }
        }
    }
}
