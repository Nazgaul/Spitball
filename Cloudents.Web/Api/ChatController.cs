using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Query.Chat;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Api
{
    [Route("api/[controller]")]
    [Authorize, ApiController]
    public class ChatController : UploadControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly UserManager<User> _userManager;

        public ChatController(ICommandBus commandBus, UserManager<User> userManager, IQueryBus queryBus,
            IChatDirectoryBlobProvider blobProvider,
            ITempDataDictionaryFactory tempDataDictionaryFactory,
            IStringLocalizer<UploadControllerBase> localizer)
        : base(blobProvider, tempDataDictionaryFactory,localizer)
        {
            _commandBus = commandBus;
            _userManager = userManager;
            _queryBus = queryBus;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<IEnumerable<ChatUserDto>> Get(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var result = await _queryBus.QueryAsync(new ChatConversationsQuery(userId), token);
            return result;
        }

        [HttpGet("conversation/{id}")]
        public async Task<ActionResult<ChatUserDto>> GetConversation(string id, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var result = await _queryBus.QueryAsync(new ChatConversationQuery(id, userId), token);
            if (result == null)
            {
                return BadRequest();
            }
            return result;
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<ChatMessageDto>> Get(string id, int page,
            [FromServices] IBinarySerializer serializer,
            CancellationToken token)
        {
            //specific conversation
            var result = await _queryBus.QueryAsync(new ChatConversationByIdQuery(id, page), token);
            return result.Select(s =>
            {
                if (!(s is ChatAttachmentDto p)) return s;
                var url = BlobProvider.GetBlobUrl($"{p.ChatRoomId}/{p.Id}/{p.Attachment}");
                p.Src = Url.ImageUrl(new ImageProperties(url), serializer);
                p.Href = Url.RouteUrl("ChatDownload", new
                {
                    chatRoomId = p.ChatRoomId,
                    chatId = p.Id
                });

                return s;
            });
        }

        // POST api/<controller>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Post([FromBody]ChatMessageRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            if (userId == model.OtherUser)
            {
                return BadRequest();
            }
            var command = new SendChatTextMessageCommand(model.Message, userId,model.OtherUser );
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPost("read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ResetUnread(ChatResetRequest model, CancellationToken token)
        {
            try
            {
                var userId = _userManager.GetLongUserId(User);
                if (userId == model.OtherUserId)
                {
                    return BadRequest();
                }
                var command = new ResetUnreadInChatCommand(userId,
                    new[] { model.OtherUserId });
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (NullReferenceException)
            {
                return BadRequest();
            }
        }

        [NonAction]
        public override async Task FinishUploadAsync(UploadRequestFinish model, string blobName, CancellationToken token)
        {
            if (model is FinishChatUpload chatModel)
            {
                var userId = _userManager.GetLongUserId(User);
                if (userId == chatModel.OtherUser)
                {
                    throw new ArgumentException();
                }
                var command = new SendChatFileMessageCommand(blobName, userId, new[] { chatModel.OtherUser });
                await _commandBus.DispatchAsync(command, token);

            }
        }

        protected override string BlobFileName(Guid sessionId, string name)
        {
            return $"file-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}-1{Path.GetExtension(name)}";
        }
    }
}
