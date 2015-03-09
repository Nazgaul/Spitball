using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    public class ItemController : ApiController
    {
        public ApiServices Services { get; set; }
        public IZboxCacheReadService ZboxReadService { get; set; }

        // GET api/Item
        public  HttpResponseMessage Get()
        {
            //var query = new GetItemQuery(userId, itemId, boxId);
            //var retVal = await ZboxReadService.GetItem2(query);

            ////var tTransAction = m_QueueProvider.InsertMessageToTranactionAsync(
            ////      new StatisticsData4(new List<StatisticsData4.StatisticItemData>
            ////        {
            ////            new StatisticsData4.StatisticItemData
            ////            {
            ////                Id = itemId,
            ////                Action = (int)StatisticsAction.View
            ////            }
            ////        }, userId, DateTime.UtcNow));

            ////await Task.WhenAll(tItem, tTransAction);
            ////var retVal = tItem.Result;
            ////retVal.UserType = ViewBag.UserType;
            ////retVal.Name = Path.GetFileNameWithoutExtension(retVal.Name);
            //return Json(new JsonResponse(true, retVal));
            return Request.CreateResponse();
        }

    }
}
