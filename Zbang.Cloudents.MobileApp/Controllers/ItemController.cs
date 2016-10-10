using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [MobileAppController]
    [Authorize]
    public class ItemController : ApiController
    {
        private readonly IZboxCacheReadService m_ZboxReadService;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IGuidIdGenerator m_GuidGenerator;
        private readonly IBlobProvider2<FilesContainerName> m_BlobProviderFiles;

        public ItemController(IZboxCacheReadService zboxReadService, IQueueProvider queueProvider, IZboxWriteService zboxWriteService, IGuidIdGenerator guidGenerator, IBlobProvider2<FilesContainerName> blobProviderFiles)
        {
            m_ZboxReadService = zboxReadService;
            m_QueueProvider = queueProvider;
            m_ZboxWriteService = zboxWriteService;
            m_GuidGenerator = guidGenerator;
            m_BlobProviderFiles = blobProviderFiles;
        }



        //[VersionedRoute("api/item", 2),
        [HttpGet]
        [Route("api/item"), ActionName("Get")]
        public async Task<HttpResponseMessage> GetAsync(long id)
        {
            var userId = User.GetUserId();
            var query = new GetItemQuery(userId, id, 0);
            var tItem = m_ZboxReadService.GetItemDetailApiAsync(query);

            var tTransAction = m_QueueProvider.InsertMessageToTranactionAsync(
                  new StatisticsData4(new List<StatisticsData4.StatisticItemData>
                    {
                        new StatisticsData4.StatisticItemData
                        {
                            Id = id,
                            Action = (int)StatisticsAction.View
                        }
                    }));

            await Task.WhenAll(tItem, tTransAction);
            var retVal = tItem.Result;
            retVal.ShortUrl = UrlConst.BuildShortItemUrl(new Base62(id).ToString());

            return Request.CreateResponse(new
            {
                retVal.BoxName,
                retVal.CreationTime,
                retVal.Id,
                retVal.Name,
                retVal.NumberOfDownloads,
                retVal.NumberOfViews,
                retVal.Owner,
                retVal.OwnerId,
                retVal.ShortUrl,
                retVal.Size,
                retVal.Source,
                retVal.Type

            });
        }

        [HttpGet]
        [Route("api/item/downloadLink")]
        public string Download(string blob)
        {
            return m_BlobProviderFiles.GenerateSharedAccessReadPermission(blob, 20);
        }

        [HttpGet]
        [Route("api/item/{id:long}/download")]
        public async Task<string> DownloadAsync(long boxId, long id)
        {
            //const string defaultMimeType = "application/octet-stream";
            var userId = User.GetUserId();

            var query = new GetItemQuery(userId, id, boxId);

            //var item = ZboxReadService.GetItem(query);
            var t2 = m_ZboxReadService.GetItemDetailApiAsync(query);


            var autoFollowCommand = new SubscribeToSharedBoxCommand(userId, boxId);
            var t3 = m_ZboxWriteService.SubscribeToSharedBoxAsync(autoFollowCommand);

            var t1 = m_QueueProvider.InsertMessageToTranactionAsync(
                   new StatisticsData4(new List<StatisticsData4.StatisticItemData>
                    {
                        new StatisticsData4.StatisticItemData
                        {
                            Id = id,
                            Action = (int)StatisticsAction.Download
                        }
                    }));

            await Task.WhenAll(t1, t2, t3);
            var item = t2.Result;
            if (item.Type == "Link")
            {
                return item.Source;
            }
            return m_BlobProviderFiles.GenerateSharedAccessReadPermission(item.Source, 120);
            //return BlobProvider.GenerateSharedAccressReadPermissionInStorage(new Uri(item.Source), 240);
            //return m_BlobUpload.GenerateReadAccessPermissionToBlob(item.Source);

        }



        [HttpDelete, Route("api/item"), ActionName("Delete")]
        public async Task<HttpResponseMessage> DeleteAsync(long id/*, long boxId*/)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var command = new DeleteItemCommand(id, User.GetUserId());
            await m_ZboxWriteService.DeleteItemAsync(command);
            return Request.CreateResponse();
        }



        [HttpGet]
        [Route("api/item/upload")]
        public string UploadLink(string blob, string mimeType)
        {
           return m_BlobProviderFiles.GenerateSharedAccessWritePermission(blob, mimeType);
            //return m_BlobUpload.GenerateWriteAccessPermissionToBlob(blob, mimeType);
        }
        // ReSharper disable once ConsiderUsingAsyncSuffix - api call
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
                        title = await helper.BringTitleAsync(model.FileUrl);
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

                var command = new AddLinkToBoxCommand(User.GetUserId(), model.BoxId, model.FileUrl, null, title,
                    model.Question);
                var result = await m_ZboxWriteService.AddItemToBoxAsync(command);
                var result2 = result as AddLinkToBoxCommandResult;
                if (result2 == null)
                {
                    throw new NullReferenceException("result2");
                }

                var item = new ItemDto
                {
                    Id = result2.Link.Id,
                    Name = result2.Link.Name,

                };
                return Request.CreateResponse(item);
            }
            catch (DuplicateNameException)
            {
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.Conflict, "Link already exists");
            }



        }
        [HttpPost]
        [Route("api/item/upload/dropbox"), ActionName("DropBox")]
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
                await m_BlobProviderFiles.UploadFromLinkAsync(model.FileUrl, blobAddressUri);
                size = await m_BlobProviderFiles.SizeAsync(blobAddressUri);
            }
            catch (UnauthorizedAccessException)
            {
                Request.CreateBadRequestResponse("can't access dropbox api");
            }

            var command = new AddFileToBoxCommand(userId, model.BoxId, blobAddressUri,
               model.Name, size, model.TabId, model.Question);
            var result = await m_ZboxWriteService.AddItemToBoxAsync(command);
            var result2 = result as AddFileToBoxCommandResult;
            if (result2 == null)
            {
                throw new NullReferenceException("result2");
            }
            var fileDto = new ItemDto
            {
                Id = result2.File.Id,
                Name = result2.File.Name,

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
            var command = new AddFileToBoxCommand(User.GetUserId(),
                boxId, model.BlobName,
                   model.FileName,
                    size, null, model.Question);
            var result = await m_ZboxWriteService.AddItemToBoxAsync(command);

            var result2 = result as AddFileToBoxCommandResult;
            if (result2 == null)
            {
                throw new NullReferenceException("result2");
            }

            var fileDto = new ItemDto
            {
                Id = result2.File.Id,
                Name = result2.File.Name,

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

            var userId = User.GetUserId();
            try
            {
                var command = new ChangeFileNameCommand(model.Id, model.Name, userId);
                var result = m_ZboxWriteService.ChangeFileName(command);
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
        // ReSharper disable once ConsiderUsingAsyncSuffix - api call
        [Route("api/item/like")]
        [HttpPost]
        public async Task<HttpResponseMessage> Like(ItemLikeRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var id = m_GuidGenerator.GetId();
            var command = new RateItemCommand(model.Id, User.GetUserId(), id, model.BoxId);
            await m_ZboxWriteService.RateItemAsync(command);

            return Request.CreateResponse();

        }

        // ReSharper disable once ConsiderUsingAsyncSuffix - api call
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
            await m_QueueProvider.InsertMessageToTranactionAsync(new BadItemData("From iOS app", string.Empty, User.GetUserId(), model.ItemId));
            return Request.CreateResponse();
        }
    }
}
