using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp2.DataObjects;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.StorageApp;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class ItemController : ApiController
    {
        public ApiServices Services { get; set; }
        public IZboxCacheReadService ZboxReadService { get; set; }

        public IQueueProvider QueueProvider { get; set; }
        public IBlobProvider BlobProvider { get; set; }
        public IZboxWriteService ZboxWriteService { get; set; }

        //public IFileProcessorFactory FileProcessorFactory { get; set; }

        public IBlobUpload BlobUpload { get; set; }
        public IGuidIdGenerator GuidGenerator { get; set; }

        // GET api/Item
        public async Task<HttpResponseMessage> Get(long boxId, long id)
        {
            var userId = User.GetCloudentsUserId();
            var query = new GetItemQuery(userId, id, boxId);
            var tItem = ZboxReadService.GetItemDetailApi(query);

            var tTransAction = QueueProvider.InsertMessageToTranactionAsync(
                  new StatisticsData4(new List<StatisticsData4.StatisticItemData>
                    {
                        new StatisticsData4.StatisticItemData
                        {
                            Id = id,
                            Action = (int)StatisticsAction.View
                        }
                    }, userId, DateTime.UtcNow));

            await Task.WhenAll(tItem, tTransAction);
            var retVal = tItem.Result;
            retVal.ShortUrl = UrlConsts.BuildShortItemUrl(new Base62(id).ToString());
            ////retVal.UserType = ViewBag.UserType;
            ////retVal.Name = Path.GetFileNameWithoutExtension(retVal.Name);

            return Request.CreateResponse(retVal);
        }

        [HttpGet]
        [Route("api/item/{id:long}/download")]
        public async Task<string> Download(long boxId, long id)
        {
            //const string defaultMimeType = "application/octet-stream";
            var userId = User.GetCloudentsUserId();

            var query = new GetItemQuery(userId, id, boxId);

            var item = ZboxReadService.GetItem(query);

            var filedto = item as FileWithDetailDto;
            if (filedto == null) // link
            {
                return item.Blob;
            }
            var autoFollowCommand = new SubscribeToSharedBoxCommand(userId, boxId);
            var t3 = ZboxWriteService.SubscribeToSharedBoxAsync(autoFollowCommand);

            var t1 = QueueProvider.InsertMessageToTranactionAsync(
                   new StatisticsData4(new List<StatisticsData4.StatisticItemData>
                    {
                        new StatisticsData4.StatisticItemData
                        {
                            Id = id,
                            Action = (int)StatisticsAction.Download
                        }
                    }, userId, DateTime.UtcNow));



            await Task.WhenAll(t1, t3);
            return BlobUpload.GenerateReadAccessPermissionToBlob(filedto.Blob);

        }



        public async Task<HttpResponseMessage> Delete(long id, long boxId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var command = new DeleteItemCommand(id, User.GetCloudentsUserId(), boxId);
            await ZboxWriteService.DeleteItemAsync(command);
            return Request.CreateResponse();
        }



        [HttpGet]
        [Route("api/item/upload")]
        public string UploadLink(string blob, string mimeType)
        {
            return BlobUpload.GenerateWriteAccessPermissionToBlob(blob, mimeType);
        }

        [HttpPost]
        [Route("api/item/upload/link")]
        public async Task<HttpResponseMessage> Link(AddLinkRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }

            try
            {

                var helper = new UrlTitleBringer();
                var title = model.Name;
                if (string.IsNullOrEmpty(title))
                {
                    try
                    {
                        title = await helper.BringTitle(model.FileUrl);
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError("on bringing title of url " + model.FileUrl, ex);

                    }
                }
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = model.FileUrl;
                }

                var command = new AddLinkToBoxCommand(User.GetCloudentsUserId(), model.BoxId, model.FileUrl, null, title,
                    model.Question);
                var result = await ZboxWriteService.AddItemToBoxAsync(command);
                var result2 = result as AddLinkToBoxCommandResult;
                if (result2 == null)
                {
                    throw new NullReferenceException("result2");
                }

                var item = new ItemDto
                {
                    Id = result2.Link.Id,
                    Name = result2.Link.Name,
                    Thumbnail = result2.Link.ThumbnailUrl,

                };
                return Request.CreateResponse(item);
            }
            catch (DuplicateNameException)
            {
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.Conflict, "Link already exists");
            }



        }

        [HttpPost]
        [Route("api/item/upload/dropbox")]
        public async Task<HttpResponseMessage> DropBox(DropboxUploadRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();

            }
            if (!ModelState.IsValid)
            {

                return Request.CreateBadRequestResponse();
            }
            var userId = User.GetCloudentsUserId();
            var extension = Path.GetExtension(model.Name);
            if (extension == null)
            {
                return Request.CreateBadRequestResponse("Can't upload file without extension");
            }
            var blobAddressUri = Guid.NewGuid().ToString().ToLower() + extension.ToLower();

            var size = 0L;

            try
            {
                size = await BlobProvider.UploadFromLinkAsync(model.FileUrl, blobAddressUri);
            }
            catch (UnauthorizedAccessException)
            {
                Request.CreateBadRequestResponse("can't access dropbox api");
            }

            var command = new AddFileToBoxCommand(userId, model.BoxId, blobAddressUri,
               model.Name, size, model.TabId, model.Question);
            var result = await ZboxWriteService.AddItemToBoxAsync(command);
            var result2 = result as AddFileToBoxCommandResult;
            if (result2 == null)
            {
                throw new NullReferenceException("result2");
            }
            var fileDto = new ItemDto
            {
                Id = result2.File.Id,
                Name = result2.File.Name,
                Thumbnail = result2.File.ThumbnailUrl,

            };

            return Request.CreateResponse(fileDto);



        }

        [Route("api/item/upload/commit")]
        [HttpPost]
        public async Task<HttpResponseMessage> CommitFile(FileUploadRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            long boxId, size;
            if (!long.TryParse(model.BoxId, out boxId))
            {
                return Request.CreateBadRequestResponse();
            }
            if (!long.TryParse(model.Size, out size))
            {
                return Request.CreateBadRequestResponse();
            }
            var command = new AddFileToBoxCommand(User.GetCloudentsUserId(),
                boxId, model.BlobName,
                   model.FileName,
                    size, null, model.Question);
            var result = await ZboxWriteService.AddItemToBoxAsync(command);

            var result2 = result as AddFileToBoxCommandResult;
            if (result2 == null)
            {
                throw new NullReferenceException("result2");
            }

            var fileDto = new ItemDto
            {
                Id = result2.File.Id,
                Name = result2.File.Name,
                Thumbnail = result2.File.ThumbnailUrl,

            };

            return Request.CreateResponse(fileDto);

        }


        [Route("api/item/rename")]
        [HttpPost]
        public HttpResponseMessage Rename(ItemRenameRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }

            var userId = User.GetCloudentsUserId();
            try
            {
                var command = new ChangeFileNameCommand(model.Id, model.Name, userId);
                var result = ZboxWriteService.ChangeFileName(command);
                return Request.CreateResponse(new
                {
                    name = result.Name,

                });
            }

            catch (UnauthorizedAccessException)
            {
                return Request.CreateBadRequestResponse("You need to follow this box in order to change file name");
            }
            catch (ArgumentException ex)
            {
                return Request.CreateBadRequestResponse(ex.Message);
            }

        }

        [Route("api/item/like")]
        [HttpPost]
        public async Task<HttpResponseMessage> Like(ItemLikeRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var id = GuidGenerator.GetId();
            var command = new RateItemCommand(model.Id, User.GetCloudentsUserId(), 5, id, model.BoxId);
            await ZboxWriteService.RateItemAsync(command);

            return Request.CreateResponse();

        }


        [Route("api/item/flag")]
        [HttpPost]
        public async Task<HttpResponseMessage> Flag(FlagItemRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            await QueueProvider.InsertMessageToTranactionAsync(new BadItemData("From iOS app", string.Empty, User.GetCloudentsUserId(), model.ItemId));
            return Request.CreateResponse();
        }
    }
}
