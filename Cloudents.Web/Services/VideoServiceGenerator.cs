using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.Interfaces;

namespace Cloudents.Web.Services
{
    public class VideoServiceGenerator : IDocumentGenerator
    {
        private readonly IVideoService _videoService;
        private readonly IUrlBuilder _urlBuilder;

        public VideoServiceGenerator(IVideoService videoService, IUrlBuilder urlBuilder)
        {
            _videoService = videoService;
            _urlBuilder = urlBuilder;
        }

        public async Task<object> GeneratePreviewAsync(DocumentDetailDto model, long userId, CancellationToken token)
        {
            string locator;
            if (model.IsPurchased)
            {
                locator = await _videoService.BuildUserStreamingLocatorAsync(model.Id, userId, token);
            }
            else
            {
                locator = await _videoService.GetShortStreamingUrlAsync(model.Id, token);
            }

            var uri = _urlBuilder.BuildDocumentThumbnailEndpoint(model.Id);


            return new { locator, poster = uri };
        }
    }
}