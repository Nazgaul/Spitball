using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Mobile.Models;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Cloudents.Mobile.Controllers
{
    [MobileAppController]
    public class DocumentController : ApiController
    {
        private readonly IBlobProvider2<FilesContainerName> _blobProviderFiles;
        private readonly IQueueProvider _queueProvider;
        private readonly IZboxWriteService _zboxWriteService;
        private readonly IZboxReadService _zboxReadService;
        private readonly IGuidIdGenerator _guidGenerator;

        public DocumentController(IBlobProvider2<FilesContainerName> blobProviderFiles, IQueueProvider queueProvider, IZboxWriteService zboxWriteService, IGuidIdGenerator guidGenerator, IZboxReadService zboxReadService)
        {
            _blobProviderFiles = blobProviderFiles;
            _queueProvider = queueProvider;
            _zboxWriteService = zboxWriteService;
            _guidGenerator = guidGenerator;
            _zboxReadService = zboxReadService;
        }

        [Route("api/document/{id}")]
        public async Task<HttpResponseMessage> GetDataAsync(long id)
        {
            var retVal = await _zboxReadService.GetItemDetailApiAsync(id).ConfigureAwait(false);

            return Request.CreateResponse(new
            {
                retVal.Name,
                retVal.Source,
            });
        }

        // GET api/Document
        public async Task<HttpResponseMessage> Get(long itemId, string blob, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(blob))
            {
                return Request.CreateBadRequestResponse();
            }
            await _queueProvider.InsertMessageToTransactionAsync(
                new StatisticsData4(
                    new StatisticsData4.StatisticItemData
                    {
                        Id = itemId,
                        Action = (int)StatisticsAction.View
                    }
                    , -1), cancellationToken).ConfigureAwait(false);
            var blobUrl = _blobProviderFiles.GenerateSharedAccessReadPermission(blob, 20);
            return Request.CreateResponse(blobUrl);
        }

        [Route("api/document/like")]
        [HttpPost]
        [Authorize]
        public async Task<HttpResponseMessage> LikeAsync(ItemLikeRequest model)
        {
            if (ModelState?.IsValid == false)
            {
                return Request.CreateBadRequestResponse();
            }
            var id = _guidGenerator.GetId();
            var command = new RateItemCommand(model.Id, User.GetUserId(), id);
            await _zboxWriteService.RateItemAsync(command).ConfigureAwait(true);

            if (model.Tags?.Any() == true)
            {
                var z = new AssignTagsToDocumentCommand(model.Id, model.Tags, TagType.User);
                await _zboxWriteService.AddItemTagAsync(z).ConfigureAwait(false);
            }

            return Request.CreateResponse();
        }

        [HttpGet]
        [Route("api/document/upload")]
        public string UploadLink(string blob, string mimeType)
        {
            return _blobProviderFiles.GenerateSharedAccessWritePermission(blob, mimeType);
        }

        [Route("api/document/upload/commit")]
        [HttpPost]
        [Authorize]
        public async Task<HttpResponseMessage> CommitFileAsync(FileUploadRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!long.TryParse(model.BoxId, out var boxId))
            {
                return Request.CreateBadRequestResponse();
            }

            var size = await _blobProviderFiles.SizeAsync(model.BlobName).ConfigureAwait(false);
            var command = new AddFileToBoxCommand(User.GetUserId(),
                boxId, model.BlobName,
                   model.FileName,
                    size, null, model.Question);
            var result = await _zboxWriteService.AddItemToBoxAsync(command).ConfigureAwait(false);

            if (!(result is AddFileToBoxCommandResult result2))
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "result is null");
            }

            var fileDto = new ItemDto
            {
                Id = result2.File.Id
                //Name = result2.File.Name

            };

            return Request.CreateResponse(fileDto);
        }

        [HttpPost]
        [Authorize]
        [Route("api/document/upload/dropbox")]
        public async Task<HttpResponseMessage> DropBoxAsync(DropboxUploadRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var userId = User.GetUserId();
            var extension = Path.GetExtension(model.Name);
            if (extension == null)
            {
                return Request.CreateBadRequestResponse("Can't upload file without extension");
            }
            var blobAddressUri = Guid.NewGuid().ToString().ToLower() + extension.ToLower();

            var size = 0L;

            try
            {
                await _blobProviderFiles.UploadFromLinkAsync(model.Url, blobAddressUri).ConfigureAwait(false);
                size = await _blobProviderFiles.SizeAsync(blobAddressUri).ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException)
            {
                Request.CreateUnauthorizedResponse("can't access dropbox api");
            }

            var command = new AddFileToBoxCommand(userId, model.BoxId, blobAddressUri,
               model.Name, size, null, model.Question);
            var result = await _zboxWriteService.AddItemToBoxAsync(command).ConfigureAwait(false);
            var result2 = result as AddFileToBoxCommandResult;
            if (result2 == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "result is null");
            }
            var fileDto = new ItemDto
            {
                Id = result2.File.Id,
                Source = blobAddressUri
                //Name = result2.File.Name,
            };
            return Request.CreateResponse(fileDto);
        }
    }
}
