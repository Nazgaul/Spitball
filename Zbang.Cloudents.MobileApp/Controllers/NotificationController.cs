using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.MobileApp.Extensions;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
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

    }
}
