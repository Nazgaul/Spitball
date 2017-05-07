using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class DocumentController : ApiController
    {
        private readonly IBlobProvider2<FilesContainerName> m_BlobProviderFiles;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IGuidIdGenerator m_GuidGenerator;


        public DocumentController(IBlobProvider2<FilesContainerName> blobProviderFiles, IQueueProvider queueProvider, IZboxWriteService zboxWriteService, IGuidIdGenerator guidGenerator)
        {
            m_BlobProviderFiles = blobProviderFiles;
            m_QueueProvider = queueProvider;
            m_ZboxWriteService = zboxWriteService;
            m_GuidGenerator = guidGenerator;
        }

        // GET api/Document
        public async Task<HttpResponseMessage> Get(long itemId, string blob, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(blob))
            {
                return Request.CreateBadRequestResponse();
            }
            await m_QueueProvider.InsertMessageToTranactionAsync(
                new StatisticsData4(
                    new StatisticsData4.StatisticItemData
                    {
                        Id = itemId,
                        Action = (int)StatisticsAction.View
                    }
                    , -1), cancellationToken).ConfigureAwait(false);
            var blobUrl = m_BlobProviderFiles.GenerateSharedAccessReadPermission(blob, 20);
            return Request.CreateResponse(blobUrl);
        }


        [Route("api/document/like")]
        [HttpPost]
        [Authorize]
        public async Task<HttpResponseMessage> LikeAsync(ItemLikeRequest model)
        {
            if (ModelState != null && !ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var id = m_GuidGenerator.GetId();
            var command = new RateItemCommand(model.Id, User.GetUserId(), id, model.BoxId);
            await m_ZboxWriteService.RateItemAsync(command).ConfigureAwait(true);

            if (!model.Tags.Any()) return Request.CreateResponse();
            var z = new AssignTagsToDocumentCommand(model.Id, model.Tags, TagType.User);
            await m_ZboxWriteService.AddItemTagAsync(z).ConfigureAwait(false);

            return Request.CreateResponse();
        }

        //[Route("api/document/tag")]
        //[HttpPost]
        ////[Authorize]
        //public HttpResponseMessage AddTag(ItemTagRequest model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateBadRequestResponse();
        //    }

        //    var z = new AssignTagsToDocumentCommand(model.Id, model.Tags, TagType.User);
        //    m_ZboxWriteService.AddItemTag(z);
        //    return Request.CreateResponse();
        //}


        [HttpGet]
        [Route("api/document/upload")]
        public string UploadLink(string blob, string mimeType)
        {
            return m_BlobProviderFiles.GenerateSharedAccessWritePermission(blob, mimeType);

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
            long boxId;
            if (!long.TryParse(model.BoxId, out boxId))
            {
                return Request.CreateBadRequestResponse();
            }
            
            var size = await m_BlobProviderFiles.SizeAsync(model.BlobName).ConfigureAwait(false);
            var command = new AddFileToBoxCommand(User.GetUserId(),
                boxId, model.BlobName,
                   model.FileName,
                    size, null, model.Question);
            var result = await m_ZboxWriteService.AddItemToBoxAsync(command).ConfigureAwait(false);

            var result2 = result as AddFileToBoxCommandResult;
            if (result2 == null)
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
                await m_BlobProviderFiles.UploadFromLinkAsync(model.Url, blobAddressUri).ConfigureAwait(false);
                size = await m_BlobProviderFiles.SizeAsync(blobAddressUri).ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException)
            {
                Request.CreateUnauthorizedResponse("can't access dropbox api");
            }

            var command = new AddFileToBoxCommand(userId, model.BoxId, blobAddressUri,
               model.Name, size, null, model.Question);
            var result = await m_ZboxWriteService.AddItemToBoxAsync(command).ConfigureAwait(false);
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
