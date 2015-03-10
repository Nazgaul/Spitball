﻿using System;
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
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class ItemController : ApiController
    {
        public ApiServices Services { get; set; }
        public IZboxCacheReadService ZboxReadService { get; set; }

        public IQueueProvider QueueProvider { get; set; }
        public IZboxWriteService ZboxWriteService { get; set; }

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


        //public async Task<HttpResponseMessage> Preview(string blobName, int index, 
        //     CancellationToken cancellationToken)
        //{
        //    Uri uri;
        //    if (!Uri.TryCreate(blobName, UriKind.Absolute, out uri))
        //    {
        //        uri = new Uri(m_BlobProvider.GetBlobUrl(blobName));
        //    }
        //    var processor = m_FileProcessorFactory.GetProcessor(uri);
        //    if (processor == null)
        //        return Request.CreateResponse(HttpStatusCode.NoContent);

        //        var retVal = await processor.ConvertFileToWebSitePreview(uri, 0, 0, index * 3, cancellationToken);
        //        if (retVal.Content == null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NoContent);

        //        }
        //        if (string.IsNullOrEmpty(retVal.ViewName))
        //        {
        //            return JsonOk(new { preview = retVal.Content.First() });
        //        }

        //        return JsonOk(new
        //        {
                   
        //        });

        //}

        public async Task<HttpResponseMessage> Delete(DeleteItemRequest model)
        {
            var command = new DeleteItemCommand(model.ItemId, User.GetCloudentsUserId(), model.BoxId);
            await ZboxWriteService.DeleteItemAsync(command);
            return Request.CreateResponse();
        }



    }
}
