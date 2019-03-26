using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Api
{
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : UploadControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly UserManager<RegularUser> _userManager;

        public ChatController(ICommandBus commandBus, UserManager<RegularUser> userManager, IQueryBus queryBus)
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

        // GET api/<controller>/5
        [HttpGet("{id:guid}")]
        public async Task<IEnumerable<ChatMessageDto>> Get(Guid id, int page, CancellationToken token)
        {
            //specific conversation
            var result = await _queryBus.QueryAsync(new ChatConversationByIdQuery(id, page), token);
            return result;
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ChatMessageRequest model, CancellationToken token)
        {
            var command = new SendMessageCommand(model.Message, _userManager.GetLongUserId(User),
                new[] { model.OtherUser }, null, null);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        //[HttpPost("upload"), FormContentType]
        //public async Task<ActionResult<UploadResponse>> BatchUploadAsync(
        //    [FromForm] UploadRequest2 model,
        //    [FromServices] UploadService.Factory factory,
        //    [FromServices] IChatDirectoryBlobProvider blobProvider,
        //    CancellationToken token)
        //{
        //    var service = factory.Invoke(blobProvider);
        //    return await service.UploadBatchAsync(model, token);
        //}

        //[HttpPost("upload",Order = 0), StartUploading]
        //public UploadResponse StartUpload([FromBody] UploadRequest model,
        //    [FromServices] UploadService.Factory factory,
        //    [FromServices] IChatDirectoryBlobProvider blobProvider,
        //    CancellationToken token)
        //{
        //    var service = factory.Invoke(blobProvider);
        //    return service.StartUpload(model);
        //}

        //[HttpPost("upload", Order = 1)]
        //public async Task<ActionResult<UploadResponse>> FinishUpload([FromBody] UploadRequest model,
        //    [FromServices] UploadService.Factory factory,
        //    [FromServices] IChatDirectoryBlobProvider blobProvider,
        //    CancellationToken token)
        //{
        //    var service = factory.Invoke(blobProvider);
        //    return await service.FinishUploadAsync(model, token);
        //}

       

        public override async Task FinishUploadAsync(UploadRequest model)
        {
            if (!(model is FinishChatUpload chatModel))
            {
               
            }
        }
    }
}
