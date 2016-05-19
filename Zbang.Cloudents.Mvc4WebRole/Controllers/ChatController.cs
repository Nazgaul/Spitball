using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [ZboxAuthorize, NoUniversity]
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ChatController : BaseController
    {
        [ActionName("UnreadCount")]
        public async Task<JsonResult> UnreadCountAsync()
        {
            var query = new QueryBase(User.GetUserId());
            var model = await ZboxReadService.GetUnreadChatMessagesAsync(query);
            return JsonOk(model);
        }


        [HttpGet, ActionName("conversation")]
        public async Task<JsonResult> ChatRoomAsync()
        {
            var model = await ZboxReadService.GetUsersWithConversationAsync(new QueryBase(User.GetUserId()));
            return JsonOk(model);
        }

        [HttpGet, ActionName("messages")]
        public async Task<JsonResult> MessagesAsync(Guid chatRoom)
        {
            var model = await ZboxReadService.GetUserConversationAsync(new GetChatRoomMessagesQuery(chatRoom));
            return JsonOk(model);
        }
        // GET: Chat

    }
}