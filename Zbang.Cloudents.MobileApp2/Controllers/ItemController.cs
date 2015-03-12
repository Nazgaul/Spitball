﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp2.DataObjects;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.StorageApp;
using Zbang.Zbox.Infrastructure.Transport;
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

        public IFileProcessorFactory FileProcessorFactory { get; set; }

        public IBlobUpload BlobUpload { get; set; }

        // GET api/Item
        public async Task<HttpResponseMessage> Get(long boxId, long itemId)
        {
            var userId = User.GetCloudentsUserId();
            var query = new GetItemQuery(userId, itemId, boxId);
            var tItem = ZboxReadService.GetItem2(query);

            var tTransAction = QueueProvider.InsertMessageToTranactionAsync(
                  new StatisticsData4(new List<StatisticsData4.StatisticItemData>
                    {
                        new StatisticsData4.StatisticItemData
                        {
                            Id = itemId,
                            Action = (int)StatisticsAction.View
                        }
                    }, userId, DateTime.UtcNow));

            await Task.WhenAll(tItem, tTransAction);
            var retVal = tItem.Result;
            ////retVal.UserType = ViewBag.UserType;
            ////retVal.Name = Path.GetFileNameWithoutExtension(retVal.Name);

            return Request.CreateResponse(new
            {
                retVal.Blob,
                retVal.Navigation.NextId,
                retVal.Navigation.PreviousId,
                //Need to add download

            });
        }

        [HttpGet]
        [Route("api/item/{itemId:long}/download")]
        public async Task<string> Download(long boxId, long itemId)
        {
            //const string defaultMimeType = "application/octet-stream";
            var userId = User.GetCloudentsUserId();

            var query = new GetItemQuery(userId, itemId, boxId);

            var item = ZboxReadService.GetItem(query);

            var filedto = item as FileWithDetailDto;
            if (filedto == null) // link
            {
                return  item.Blob;
            }
            var autoFollowCommand = new SubscribeToSharedBoxCommand(userId, boxId);
            var t3 = ZboxWriteService.SubscribeToSharedBoxAsync(autoFollowCommand);

            var t1 = QueueProvider.InsertMessageToTranactionAsync(
                   new StatisticsData4(new List<StatisticsData4.StatisticItemData>
                    {
                        new StatisticsData4.StatisticItemData
                        {
                            Id = itemId,
                            Action = (int)StatisticsAction.Download
                        }
                    }, userId, DateTime.UtcNow));



            await Task.WhenAll(t1,  t3);
            return BlobUpload.GenerateReadAccessPermissionToBlob(filedto.Blob);
            
        }

        [HttpGet]
        [Route("api/item/{itemId:long}/preview")]
        public async Task<HttpResponseMessage> Preview(string blobName, int index,
             CancellationToken cancellationToken, string itemId)
        {
            Uri uri;
            if (!Uri.TryCreate(blobName, UriKind.Absolute, out uri))
            {
                uri = new Uri(BlobProvider.GetBlobUrl(blobName));
            }
            var processor = FileProcessorFactory.GetProcessor(uri);
            if (processor == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);

            var retVal = await processor.ConvertFileToWebSitePreview(uri, 0, 0, index * 3, cancellationToken);
            if (retVal.Content == null)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent);

            }
            if (string.IsNullOrEmpty(retVal.ViewName))
            {
                return Request.CreateResponse(new { preview = retVal.Content.First() });
            }

            return Request.CreateResponse(new { preview = retVal.Content.Take(3) });

        }

        public async Task<HttpResponseMessage> Delete(DeleteItemRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var command = new DeleteItemCommand(model.ItemId, User.GetCloudentsUserId(), model.BoxId);
            await ZboxWriteService.DeleteItemAsync(command);
            return Request.CreateResponse();
        }

        //[HttpPost]
        //[Route("api/item/upload")]
        //public async Task<HttpResponseMessage> Upload()
        //{
        //    var userId = User.GetCloudentsUserId();
        //    //var cookie = new CookieHelper(HttpContext);
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }
        //    var provider = new InMemoryMultipartFormDataStreamProvider();
        //    await Request.Content.ReadAsMultipartAsync(provider);



        //    var uploadedfile = provider.Files[0];
        //    if (uploadedfile == null) throw new NullReferenceException("uploadedfile");

        //    var blobGuid = provider.FormData["blobName"];
        //    var extension = Path.GetExtension(provider.FormData["fileName"]);
        //    var index = int.Parse(provider.FormData["index"]);

        //    string blobAddressUri = blobGuid.ToLower() + extension.ToLower();
        //    await BlobProvider.UploadFileBlockAsync(blobAddressUri, await uploadedfile.ReadAsStreamAsync(), index);

        //    return Request.CreateResponse();
        //}

        [HttpGet]
        [Route("api/item/upload")]
        public string UploadLink(string blob, string mimeType)
        {
            return BlobUpload.GenerateWriteAccessPermissionToBlob(blob, mimeType);
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
                    size, null, false);
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

    }
}
