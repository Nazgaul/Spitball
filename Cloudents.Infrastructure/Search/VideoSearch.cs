using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Google.Apis.Services;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Search
{
    public class VideoSearch:IVideoSearch
    {
        public async Task<VideoDto> SearchAsync(string term, CancellationToken token)
        {
            var t = new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyCAaZGgVHm0GxY2lY_mWQw3JXGy7KMypZ0"
            });
            var client = new Google.Apis.YouTube.v3.SearchResource(t);

            var query = client.List("snippet");
            query.Q = term;
            query.Type = "video";
            query.VideoDuration = Google.Apis.YouTube.v3.SearchResource.ListRequest.VideoDurationEnum.Short__;
            query.VideoEmbeddable = Google.Apis.YouTube.v3.SearchResource.ListRequest.VideoEmbeddableEnum.True__;
            query.VideoSyndicated = Google.Apis.YouTube.v3.SearchResource.ListRequest.VideoSyndicatedEnum.True__;
            query.VideoCategoryId = "27";
            query.MaxResults = 1;

            var result = await query.ExecuteAsync(token).ConfigureAwait(false);
            var videoId = result.Items.FirstOrDefault()?.Id.VideoId;
            var url="";
            if(!string.IsNullOrEmpty(videoId))
            {
                url =$"https://www.youtube.com/embed/{videoId}";
            }

            return new VideoDto
            {
                Url = url
            };
        }
    }
}
