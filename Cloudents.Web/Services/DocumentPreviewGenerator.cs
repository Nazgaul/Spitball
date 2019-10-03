using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;

namespace Cloudents.Web.Services
{
    public class DocumentPreviewGenerator : IDocumentGenerator
    {
        private readonly IDocumentDirectoryBlobProvider _blobProvider;
        private readonly LinkGenerator _linkGenerator;
        private readonly IBinarySerializer _binarySerializer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DocumentPreviewGenerator(IDocumentDirectoryBlobProvider blobProvider, IBinarySerializer binarySerializer, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _blobProvider = blobProvider;
            _binarySerializer = binarySerializer;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<object> GeneratePreview(DocumentDetailDto model, long userId, CancellationToken token)
        {
            var result = Enumerable.Range(0, model.Pages).Select(i =>
            {
                var uri = _blobProvider.GetPreviewImageLink(model.Id, i);
                var effect = ImageProperties.BlurEffect.None;
                if (!model.IsPurchased)
                {
                    effect = ImageProperties.BlurEffect.All;
                    if (i == 0)
                    {
                        effect = ImageProperties.BlurEffect.Part;
                    }

                }

                var properties = new ImageProperties(uri, effect);
                var hash = _binarySerializer.Serialize(properties);

                return _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, "imageUrl", new
                {
                    hash = Base64UrlTextEncoder.Encode(hash)
                });
            });

            return Task.FromResult<object>(result);
        }
    }


    public class VideoServiceGenerator : IDocumentGenerator
    {
        private readonly IVideoService _videoService;
        private readonly IUrlBuilder _urlBuilder;

        public VideoServiceGenerator(IVideoService videoService, IUrlBuilder urlBuilder)
        {
            _videoService = videoService;
            _urlBuilder = urlBuilder;
        }

        public async Task<object> GeneratePreview(DocumentDetailDto model, long userId, CancellationToken token)
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

    public interface IDocumentGenerator
    {
        Task<object> GeneratePreview(DocumentDetailDto model, long userId, CancellationToken token);
    }
}
