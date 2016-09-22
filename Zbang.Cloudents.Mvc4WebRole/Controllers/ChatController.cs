using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [ZboxAuthorize, NoUniversity]
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ChatController : BaseController
    {
        private readonly IBlobProvider2<ChatContainerName> m_BlobProviderChat;
        private readonly IFileProcessorFactory m_FileProcessorFactory;


        


        public ChatController(IBlobProvider2<ChatContainerName> blobProviderChat, IFileProcessorFactory fileProcessorFactory)
        {
            m_BlobProviderChat = blobProviderChat;
            m_FileProcessorFactory = fileProcessorFactory;
            // m_CacheBlobProvider = cacheBlobProvider;
        }

        public ActionResult Index()
        {
            return View("Empty");
        }

        public ActionResult IndexPartial()
        {
            return PartialView("Index");
        }

        [HttpGet, ActionName("conversation")]
        public async Task<JsonResult> ChatRoomAsync(string q, int page)
        {
            var model = await ZboxReadService.GetUsersConversationAndFriendsAsync(
                new GetUserConversationAndFriends(User.GetUserId(), User.GetUniversityId().Value, q, page, 30));
            return JsonOk(model);
        }

        [HttpGet, ActionName("messages")]
        public async Task<JsonResult> MessagesAsync(ChatMessages message)
        {
            var query = new GetChatRoomMessagesQuery(message.ChatRoom, message.UserIds, message.StartTime, message.Top);
            var model = await ZboxReadService.GetUserConversationAsync(query);
            return JsonOk(model);
        }

        [HttpGet, ActionName("unread")]
        public async Task<JsonResult> UnreadAsync()
        {
            var query = new GetUserDetailsQuery(User.GetUserId());
            var model = await ZboxReadService.GetChatUnreadMessagesAsync(query);
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

        public ActionResult Download(string blobName)
        {
            var str = m_BlobProviderChat.GenerateSharedAccressReadPermission(blobName, 30);
            return Redirect(str);
        }

        [HttpGet, ActionName("Preview")]
        [AsyncTimeout(TimeConst.Minute * 3 * 1000)]
        [JsonHandleError(HttpStatus = HttpStatusCode.BadRequest, ExceptionType = typeof(ArgumentException))]
        public async Task<JsonResult> PreviewAsync(string blobName, int index, CancellationToken cancellationToken)
        {
            Uri uri;
            if (!Uri.TryCreate(blobName, UriKind.Absolute, out uri))
            {
                uri = m_BlobProviderChat.GetBlobUrl(blobName);
            }
            var processor = m_FileProcessorFactory.GetProcessor<PreviewChatContainerName, ChatCacheContainerName>(uri);
            if (processor == null)
                return JsonOk();
            try
            {
                var retVal = await processor.ConvertFileToWebSitePreviewAsync(uri, index * 3, cancellationToken);
                if (retVal.Content == null)
                {
                    return JsonOk();
                }
                if (!retVal.Content.Any())
                {
                    return JsonOk();
                }
                if (string.IsNullOrEmpty(retVal.ViewName))
                {
                    return JsonOk(new { preview = retVal.Content.First() });
                }

                return JsonOk(retVal);

            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"GeneratePreview filename: {blobName}", ex);
                if (index == 0)
                {
                    return JsonOk();
                }
                return JsonOk();
            }
        }


    }
}