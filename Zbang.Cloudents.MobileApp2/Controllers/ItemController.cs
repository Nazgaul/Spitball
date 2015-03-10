using System;
using System.Collections.Generic;
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
        public async Task<HttpResponseMessage> Download(long boxId, long itemId)
        {
            //const string defaultMimeType = "application/octet-stream";
            var userId = User.GetCloudentsUserId();

            var query = new GetItemQuery(userId, itemId, boxId);

            var item = ZboxReadService.GetItem(query);

            var filedto = item as FileWithDetailDto;
            if (filedto == null) // link
            {
                return Request.CreateResponse(new { url = item.Blob });
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


            var t2 = BlobProvider.DownloadFileAsync2(filedto.Blob, CancellationToken.None);

            await Task.WhenAll(t1, t2, t3);
            var response = Request.CreateResponse();
            response.Content = new StreamContent(t2.Result);
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = item.Name
            };

            return response;
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
            var command = new DeleteItemCommand(model.ItemId, User.GetCloudentsUserId(), model.BoxId);
            await ZboxWriteService.DeleteItemAsync(command);
            return Request.CreateResponse();
        }



    }
}
