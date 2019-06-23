using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Query;
using Cloudents.Query.Chat;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminConversationController : ControllerBase
    {
        private readonly IQueryBus _queryBus;

        public AdminConversationController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet]
        public async Task<IEnumerable<ConversationDto>> ConversationAsync([FromQuery] ConversationDetailsRequest request
            , CancellationToken token)
        {

            var query = new AdminConversationsQuery(request.Id.GetValueOrDefault(), request.Page, request.Status, request.AssignTo);
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
            [FromServices] ICommandBus commandBus,
            CancellationToken token)

        {
            if (string.IsNullOrEmpty(identifier))
            {
                return BadRequest();
            }
            var command = new ChangeConversationStatusCommand(identifier, model.Status);
            await commandBus.DispatchAsync(command, token);
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
    }
}
