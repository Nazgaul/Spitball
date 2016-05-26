using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
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
        private readonly IBlobProvider2<ChatCacheContainerName> m_CacheBlobProvider;
        private readonly IFileProcessorFactory m_FileProcessorFactory;


        

        public ChatController(IBlobProvider2<ChatContainerName> blobProviderChat, IFileProcessorFactory fileProcessorFactory, IBlobProvider2<ChatCacheContainerName> cacheBlobProvider)
        {
            m_BlobProviderChat = blobProviderChat;
            m_FileProcessorFactory = fileProcessorFactory;
            m_CacheBlobProvider = cacheBlobProvider;
        }

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


       
        [HttpGet, ActionName("Preview")]
        [AsyncTimeout(TimeConst.Minute * 3 * 1000)]
        [JsonHandleError(HttpStatus = HttpStatusCode.BadRequest, ExceptionType = typeof(ArgumentException))]
        public async Task<JsonResult> PreviewAsync(string blobName, int index, long id,
            long boxId, CancellationToken cancellationToken, int width = 0, int height = 0)
        {
            Uri uri;
            if (!Uri.TryCreate(blobName, UriKind.Absolute, out uri))
            {
                uri = new Uri(m_BlobProviderChat.GetBlobUrl(blobName));
            }
            var processor = m_FileProcessorFactory.GetProcessor<PreviewChatContainerName, ChatCacheContainerName>(uri);
            if (processor == null)
                return
                    JsonOk(
                        new
                        {
                            preview =
                                RenderRazorViewToString("_PreviewFailed",
                                    Url.RouteUrl("ItemDownload2", new { boxId, itemId = id }))
                        });

            try
            {
                var retVal = await processor.ConvertFileToWebSitePreviewAsync(uri, index * 3, cancellationToken);
                if (retVal.Content == null)
                {
                    return JsonOk(new
                    {
                        preview = RenderRazorViewToString("_PreviewFailed",
                            Url.RouteUrl("ItemDownload2", new { boxId, itemId = id }))
                    });

                }
                if (!retVal.Content.Any())
                {
                    return JsonOk();
                }
                if (string.IsNullOrEmpty(retVal.ViewName))
                {
                    return JsonOk(new { preview = retVal.Content.First() });
                }


                return JsonOk(new
                {
                    preview = RenderRazorViewToString("_Preview" + retVal.ViewName,
                     new ItemPreview(
                           retVal.Content.Take(3),
                           index,
                           User.Identity.IsAuthenticated))
                });

            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"GeneratePreview filename: {blobName}", ex);
                if (index == 0)
                {
                    return JsonOk(new
                    {
                        preview = RenderRazorViewToString("_PreviewFailed",
                            Url.RouteUrl("ItemDownload2", new { boxId, itemId = id }))
                    });
                }
                return JsonOk();
            }
        }
       

    }
}