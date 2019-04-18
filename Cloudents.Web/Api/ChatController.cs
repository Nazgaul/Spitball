using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Query;
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
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Api
{
    [Route("api/[controller]")]
    [Authorize, ApiController]
    public class ChatController : UploadControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly UserManager<RegularUser> _userManager;




        public ChatController(ICommandBus commandBus, UserManager<RegularUser> userManager, IQueryBus queryBus,
            IChatDirectoryBlobProvider blobProvider,
            ITempDataDictionaryFactory tempDataDictionaryFactory)
        : base(blobProvider, tempDataDictionaryFactory)
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

        [HttpGet("conversation/{id:guid}")]
        public async Task<ChatUserDto> GetConversation(Guid id, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var result = await _queryBus.QueryAsync(new ChatConversationQuery(id, userId), token);
            return result;
        }

        // GET api/<controller>/5
        [HttpGet("{id:guid}")]
        public async Task<IEnumerable<ChatMessageDto>> Get(Guid id, int page,
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
        public async Task<IActionResult> Post([FromBody]ChatMessageRequest model, CancellationToken token)
        {
            var command = new SendChatTextMessageCommand(model.Message, _userManager.GetLongUserId(User),
                new[] { model.OtherUser });
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
                var command = new ResetUnreadInChatCommand(_userManager.GetLongUserId(User),
                    new[] { model.OtherUser });
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
                var command = new SendChatFileMessageCommand(blobName, _userManager.GetLongUserId(User), new[] { chatModel.OtherUser });
                await _commandBus.DispatchAsync(command, token);

            }
        }

        protected override string BlobFileName(Guid sessionId, string name)
        {
            return $"file-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}-1{Path.GetExtension(name)}";
        }
    }
}
