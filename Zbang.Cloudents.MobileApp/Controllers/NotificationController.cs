using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Cloudents.MobileApp.Extensions;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [MobileAppController]
    [Authorize]
    public class NotificationController : ApiController
    {
        private readonly IZboxCacheReadService m_ZboxReadService;
        private readonly IZboxWriteService m_ZboxWriteService;

        public NotificationController(IZboxCacheReadService zboxReadService, IZboxWriteService zboxWriteService)
        {
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
        }

        // GET api/Notification
        public async Task<HttpResponseMessage> Get()
        {
            var model = await m_ZboxReadService.GetUpdatesAsync(new QueryBase(User.GetCloudentsUserId()));
            return Request.CreateResponse(model.Where(w => w.QuizId == null));
        }

        public HttpResponseMessage Delete(long boxId)
        {
            var userId = User.GetCloudentsUserId();
            var command = new DeleteUpdatesCommand(userId, boxId);
            m_ZboxWriteService.DeleteUpdates(command);
            return Request.CreateResponse();
        }

        [HttpDelete]
        [Route("api/Notification/{boxId:long}/items")]
        public HttpResponseMessage DeleteItems(long boxId)
        {
            var userId = User.GetCloudentsUserId();
            var command = new DeleteUpdatesItemCommand(userId, boxId);
            m_ZboxWriteService.DeleteUpdates(command);
            return Request.CreateResponse();
        }

        [HttpDelete]
        [Route("api/Notification/{boxId:long}/feed/{feedId:guid}")]
        public HttpResponseMessage DeleteFeed(long boxId, Guid feedId)
        {
            var userId = User.GetCloudentsUserId();
            var command = new DeleteUpdatesFeedCommand(userId, boxId, feedId);
            m_ZboxWriteService.DeleteUpdates(command);
            return Request.CreateResponse();
        }

        [HttpPut]
        [Route("api/push/apple")]
        public async Task<HttpResponseMessage> Apple([FromBody]RegisterDeviceRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(ConfigFetcher.Fetch("MS_NotificationHubConnectionString"), ConfigFetcher.Fetch("MS_NotificationHubName"));

            var registrations = await hub.GetRegistrationsByTagAsync(User.GetCloudentsUserId().ToString(), 10);
            var tasks = new List<Task>();
            foreach (var registration in registrations)
            {
                tasks.Add(hub.DeleteRegistrationAsync(registration));
            }
            await Task.WhenAll(tasks);
            var val = await hub.CreateAppleNativeRegistrationAsync(model.DeviceToken, new[] { User.GetCloudentsUserId().ToString() });
            var command = new RegisterMobileDeviceCommand(User.GetCloudentsUserId(), MobileOperatingSystem.iOS);
            m_ZboxWriteService.RegisterMobileDevice(command);
            //var installation = new Installation();
            //installation.InstallationId = deviceUpdate.InstallationId;
            //installation.PushChannel = deviceUpdate.Handle;
            //installation.Tags = deviceUpdate.Tags;

            //switch (deviceUpdate.Platform)
            //{
            //    case "mpns":
            //        installation.Platform = NotificationPlatform.Mpns;
            //        break;
            //    case "wns":
            //        installation.Platform = NotificationPlatform.Wns;
            //        break;
            //    case "apns":
            //        installation.Platform = NotificationPlatform.Apns;
            //        break;
            //    case "gcm":
            //        installation.Platform = NotificationPlatform.Gcm;
            //        break;
            //    default:
            //        throw new HttpResponseException(HttpStatusCode.BadRequest);
            //}


            //// In the backend we can control if a user is allowed to add tags
            ////installation.Tags = new List<string>(deviceUpdate.Tags);
            ////installation.Tags.Add("username:" + username);

            //await hub.CreateOrUpdateInstallationAsync(installation);

            return Request.CreateResponse(HttpStatusCode.OK);


        }

    }
}
