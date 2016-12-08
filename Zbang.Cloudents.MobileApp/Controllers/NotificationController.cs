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
            var model = await m_ZboxReadService.GetUpdatesAsync(new QueryBase(User.GetUserId()));
            return Request.CreateResponse(model.Where(w => w.QuizId == null));
        }

        public HttpResponseMessage Delete(long boxId)
        {
            var userId = User.GetUserId();
            var command = new DeleteUpdatesCommand(userId, boxId);
            m_ZboxWriteService.DeleteUpdates(command);
            return Request.CreateResponse();
        }

        [HttpDelete]
        [Route("api/Notification/{boxId:long}/items")]
        public HttpResponseMessage DeleteItems(long boxId)
        {
            var userId = User.GetUserId();
            var command = new DeleteUpdatesItemCommand(userId, boxId);
            m_ZboxWriteService.DeleteUpdates(command);
            return Request.CreateResponse();
        }

        [HttpDelete]
        [Route("api/Notification/{boxId:long}/feed/{feedId:guid}")]
        public HttpResponseMessage DeleteFeed(long boxId, Guid feedId)
        {
            var userId = User.GetUserId();
            var command = new DeleteUpdatesCommand(userId, boxId);
            m_ZboxWriteService.DeleteUpdates(command);
            return Request.CreateResponse();
        }

        [HttpPut]
        [Route("api/push/apple")]
        public async Task<HttpResponseMessage> AppleAsync([FromBody]RegisterDeviceRequest model)
        {

            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var userId = User.GetUserId();
            if (userId < 0)
            {
                return Request.CreateUnauthorizedResponse();
            }
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(ConfigFetcher.Fetch("MS_NotificationHubConnectionString"), ConfigFetcher.Fetch("MS_NotificationHubName"));

            var registrations = await hub.GetRegistrationsByTagAsync(User.GetUserId().ToString(), 10);
            var tasks = new List<Task>();
            foreach (var registration in registrations)
            {
                tasks.Add(hub.DeleteRegistrationAsync(registration));
            }
            await Task.WhenAll(tasks);
            await hub.CreateAppleNativeRegistrationAsync(model.DeviceToken, new[] { User.GetUserId().ToString() });
            var command = new RegisterMobileDeviceCommand(User.GetUserId(), MobileOperatingSystem.iOS);
            m_ZboxWriteService.RegisterMobileDevice(command);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPut]
        [Route("api/push/google")]
        public async Task<HttpResponseMessage> GoogleAsync([FromBody] RegisterDeviceRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var userId = User.GetUserId();
            if (userId < 0)
            {
                return Request.CreateUnauthorizedResponse();
            }
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(ConfigFetcher.Fetch("MS_NotificationHubConnectionString"), ConfigFetcher.Fetch("MS_NotificationHubName"));

            var registrations = await hub.GetRegistrationsByTagAsync(User.GetUserId().ToString(), 10);
            var tasks = new List<Task>();
            foreach (var registration in registrations)
            {
                tasks.Add(hub.DeleteRegistrationAsync(registration));
            }
            await Task.WhenAll(tasks);
            await hub.CreateGcmNativeRegistrationAsync(model.DeviceToken, new[] { User.GetUserId().ToString() });
            var command = new RegisterMobileDeviceCommand(User.GetUserId(), MobileOperatingSystem.Android);
            m_ZboxWriteService.RegisterMobileDevice(command);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
