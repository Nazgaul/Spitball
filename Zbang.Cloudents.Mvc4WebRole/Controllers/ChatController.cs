using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [ZboxAuthorize, NoUniversity]
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ChatController : BaseController
    {
        //[ActionName("UnreadCount")]
        //public async Task<JsonResult> UnreadCountAsync()
        //{
        //    var query = new QueryBase(User.GetUserId());
        //    var model = await ZboxReadService.GetUnreadChatMessagesAsync(query);
        //    return JsonOk(model);
        //}


        [HttpGet, ActionName("conversation")]
        public async Task<JsonResult> ChatRoomAsync(string q)
        {
            var model = await ZboxReadService.GetUsersConversationAndFriendsAsync(new GetUserConversationAndFriends(User.GetUserId(), User.GetUniversityId().Value, q));
            return JsonOk(model);
        }

        [HttpGet, ActionName("messages")]
        public async Task<JsonResult> MessagesAsync(ChatMessages message)
        {
            var model = await ZboxReadService.GetUserConversationAsync(new GetChatRoomMessagesQuery(message.ChatRoom, message.UserIds));
            return JsonOk(model);
        }

        [HttpPost]
        public JsonResult MarkRead(Guid chatRoom)
        {
            var command = new ChatMarkAsReadCommand(User.GetUserId(), chatRoom);
            ZboxWriteService.MarkChatAsRead(command);
            return JsonOk();
        }


        [HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult PreviewDialog()
        {
            return PartialView();
        }
        // GET: Chat

    }
}