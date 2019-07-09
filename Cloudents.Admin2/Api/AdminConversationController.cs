using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Cloudents.Query;
using Cloudents.Query.Chat;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminConversationController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public AdminConversationController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet]
        public async Task<IEnumerable<ConversationDto>> ConversationAsync([FromQuery] ConversationDetailsRequest request
            , CancellationToken token)
        {
            var query = new AdminConversationsQuery(request.Id.GetValueOrDefault(), request.Page, 
                request.Status, request.AssignTo, request.AutoStatus);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("{identifier}/details")]
        public async Task<IEnumerable<ConversationDetailsDto>> ConversationDetailAsync(
           [FromRoute] string identifier,
            CancellationToken token)
        {
            var query = new AdminConversationDetailsQuery(identifier);
            return await _queryBus.QueryAsync(query, token);
        }


        [HttpGet("{identifier}")]
//        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = TimeConst.Hour, VaryByQueryKeys = new []{ "*" })]
        public async Task<IEnumerable<ChatMessageDto>> Get(string identifier,
            CancellationToken token)
        {
            var result = await _queryBus.QueryAsync(new ChatConversationByIdQuery(identifier, 0), token);
            return result;
        }

        [HttpPost("{identifier}/status")]
        public async Task<IActionResult> ChangeStatus(
            [FromRoute] string identifier,
            ChangeConversationStatusRequest model,

            CancellationToken token)

        {
            if (string.IsNullOrEmpty(identifier))
            {
                return BadRequest();
            }
            var command = new ChangeConversationStatusCommand(identifier, model.Status);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }


        [HttpPost("start")]
        public async Task<IActionResult> StartConversation(StartConversationRequest model,
            CancellationToken token)
        {
            var command = new SendChatTextMessageCommand(model.Message, model.UserId,
                model.TutorId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();

        }

        [HttpPost("{identifier}/assignTo")]
        public async Task<IActionResult> ChangeAssign(
            [FromRoute] string identifier,
            ChangeConversationAssignRequest model,
            [FromServices] ICommandBus commandBus,
            CancellationToken token)

        {
            if (string.IsNullOrEmpty(identifier))
            {
                return BadRequest();
            }
            var command = new ChangeConversationAssignCommand(identifier, model.AssignTo);
            await commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpGet("params")]
        public ConversationParamsResponse GetParams()
        {
            return new ConversationParamsResponse()
            {
                Status = Enum.GetNames(typeof(ChatRoomStatus)).Select(s=> s.ToCamelCase()),
                AssignTo = Enum.GetNames(typeof(ChatRoomAssign)).Select(s => s.ToCamelCase()),
                WaitingFor = Enum.GetNames(typeof(WaitingFor)).Select(s => s.ToCamelCase())
            };
        }
    }
}
