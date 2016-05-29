using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Cloudents.MobileApp.Extensions;
using Zbang.Cloudents.MobileApp.Filters;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [MobileAppController]
    [ZboxAuthorize]
    public class ChatController : ApiController
    {
        private readonly IZboxCacheReadService m_ZboxReadService;
        private readonly IZboxWriteService m_ZboxWriteService;

        public ChatController(IZboxCacheReadService zboxReadService, IZboxWriteService zboxWriteService)
        {
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
        }

        [Route("api/chat")]
        [HttpGet]
        public async Task<HttpResponseMessage> ChatRoomAsync(string q)
        {
            var model = await m_ZboxReadService.GetUsersConversationAndFriendsAsync(new GetUserConversationAndFriends(User.GetCloudentsUserId(), User.GetUniversityId().Value, q));
            return Request.CreateResponse(model.Select(s => new
            {
                s.Conversation,
                s.Id,
                s.Image,
                s.LastSeen,
                s.Name,
                s.Online,
                s.Unread
            }));
        }

        [Route("api/chat/message")]
        [HttpGet]
        public async Task<HttpResponseMessage> MessagesAsync(Guid? chatRoom, IEnumerable<long> userIds, Guid? fromId, int top, int skip)
        {
            var query = new GetChatRoomMessagesQuery(chatRoom, userIds, fromId, top, skip);
            var model = await m_ZboxReadService.GetUserConversationAsync(query);
            return Request.CreateResponse(model);
        }

        [HttpPost]
        [Route("api/chat/markread")]
        public HttpResponseMessage MarkRead(MarkReadRequest request)
        {
            var command = new ChatMarkAsReadCommand(User.GetCloudentsUserId(), request.ChatRoom);
            m_ZboxWriteService.MarkChatAsRead(command);
            return Request.CreateResponse();
        }
    }
}
