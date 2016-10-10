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
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
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
        private readonly IBlobProvider2<ChatContainerName> m_ChatBlobProvider;
        private readonly Lazy<IQueueProvider> m_QueueProvider;

        public ChatController(IZboxCacheReadService zboxReadService, IZboxWriteService zboxWriteService, IBlobProvider2<ChatContainerName> chatBlobProvider, Lazy<IQueueProvider> queueProvider)
        {
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
            m_ChatBlobProvider = chatBlobProvider;
            m_QueueProvider = queueProvider;
        }

        [Route("api/chat")]
        [HttpGet]
        public async Task<HttpResponseMessage> ChatRoomAsync(string q, int page = 0, int sizePerPage = 30)
        {
            var model = await m_ZboxReadService.GetUsersConversationAndFriendsAsync(new GetUserConversationAndFriends(User.GetUserId(), User.GetUniversityId().Value, q, page, sizePerPage));
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
        public async Task<HttpResponseMessage> MessagesAsync(Guid? chatRoom, [FromUri] IEnumerable<long> userIds, DateTime? fromId, int top)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Request.CreateUnauthorizedResponse();
            }
            if (!chatRoom.HasValue && userIds == null)
            {
                return Request.CreateBadRequestResponse();
            }
            var userIdsList = userIds.ToList();
            userIdsList.Add(User.GetUserId());
            var query = new GetChatRoomMessagesQuery(chatRoom, userIdsList.Distinct(), fromId, top);
            var model = await m_ZboxReadService.GetUserConversationAsync(query);
            return Request.CreateResponse(model);
        }

        [HttpPost]
        [Route("api/chat/markread")]
        public HttpResponseMessage MarkRead(MarkReadRequest request)
        {
            var command = new ChatMarkAsReadCommand(User.GetUserId(), request.ChatRoom);
            m_ZboxWriteService.MarkChatAsRead(command);
            return Request.CreateResponse();
        }


        [HttpGet]
        [Route("api/chat/upload")]
        public string UploadLink(string blob, string mimeType)
        {
            return m_ChatBlobProvider.GenerateSharedAccessWritePermission(blob, mimeType);
        }

        [Route("api/chat/upload/commit")]
        [HttpPost]
        public async Task<HttpResponseMessage> CommitFileAsync(ChatFileUploadRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            if (model.Users.Count == 0)
            {
                return Request.CreateBadRequestResponse();
            }
            var uri = m_ChatBlobProvider.GetBlobUrl(model.BlobName);
            if (!model.Users.Contains(User.GetUserId()))
            {
                model.Users.Add(User.GetUserId());
            }
            await m_QueueProvider.Value.InsertMessageToThumbnailAsync(new ChatFileProcessData(uri, model.Users));
            return Request.CreateResponse();
        }

        [Route("api/chat/file")]
        [HttpGet]
        public HttpResponseMessage Download(string blobName)
        {
            var uri = m_ChatBlobProvider.GenerateSharedAccessReadPermission(blobName, 30);
            return Request.CreateResponse(uri);
        }



    }
}
