using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class NotificationController : ApiController
    {
        public ApiServices Services { get; set; }
        public IZboxCacheReadService ZboxReadService { get; set; }
        public IZboxWriteService ZboxWriteService { get; set; }

        // GET api/Notification
        public async Task<HttpResponseMessage> Get()
        {
            var model = await ZboxReadService.GetUpdates(new QueryBase(User.GetCloudentsUserId()));
            return Request.CreateResponse(model.Where(w => w.QuizId == null));
        }

        public HttpResponseMessage Delete(long boxId)
        {
            var userId = User.GetCloudentsUserId();
            var command = new DeleteUpdatesCommand(userId, boxId);
            ZboxWriteService.DeleteUpdates(command);
            return Request.CreateResponse();
        }

        [HttpDelete]
        [Route("api/Notification/{boxId:long}/items")]
        public HttpResponseMessage DeleteItems(long boxId)
        {
            var userId = User.GetCloudentsUserId();
            var command = new DeleteUpdatesItemCommand(userId, boxId);
            ZboxWriteService.DeleteUpdates(command);
            return Request.CreateResponse();
        }

        [HttpDelete]
        [Route("api/Notification/{boxId:long}/feed/{feedId:guid}")]
        public HttpResponseMessage DeleteFeed(long boxId, Guid feedId)
        {
            var userId = User.GetCloudentsUserId();
            var command = new DeleteUpdatesFeedCommand(userId, boxId, feedId);
            ZboxWriteService.DeleteUpdates(command);
            return Request.CreateResponse();
        }

    }
}
