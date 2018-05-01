using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Google.Apis.Services;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Read;
using Google.Apis.YouTube.v3;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Search
{
    [UsedImplicitly]
    public sealed class VideoSearch : IVideoSearch, IDisposable
    {
        private readonly YouTubeService _service;

        public VideoSearch()
        {
            _service = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyCAaZGgVHm0GxY2lY_mWQw3JXGy7KMypZ0"
            });
        }

        public async Task<VideoDto> SearchAsync(IEnumerable<string> term, CancellationToken token)
        {
            var termStr = string.Join(" ", term ?? Enumerable.Empty<string>());
            if (string.IsNullOrWhiteSpace(termStr))
            {
                termStr = CustomApiKey.AskQuestion.DefaultPhrase;
            }

            
            var client = new SearchResource(_service);

            var query = client.List("snippet");
            query.Q = termStr;
            query.Type = "video";
            query.VideoDuration = SearchResource.ListRequest.VideoDurationEnum.Short__;
            query.VideoEmbeddable = SearchResource.ListRequest.VideoEmbeddableEnum.True__;
            query.VideoSyndicated = SearchResource.ListRequest.VideoSyndicatedEnum.True__;
            query.VideoCategoryId = "27";
            query.MaxResults = 1;

            var result = await query.ExecuteAsync(token).ConfigureAwait(false);
            var videoId = result.Items.FirstOrDefault()?.Id.VideoId;
            //var url="";
            if (string.IsNullOrEmpty(videoId))
            {
                return null;
                //url =$"https://www.youtube.com/embed/{videoId}";
            }

            return new VideoDto
            {
                Url = $"https://www.youtube.com/embed/{videoId}"
            };
        }

        public void Dispose()
        {
            _service?.Dispose();
        }
    }
}
