using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Chat;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;

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
        public async Task<IEnumerable<ConversationDto>> ConversationAsync(CancellationToken token)
        {

            var query = new AdminConversationsQuery(0);
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
            int page = 0;
            var result = await _queryBus.QueryAsync(new ChatConversationByIdQuery(identifier, page), token);
            return result;
        }

        [HttpPost("{id}/status")]
        public async Task<IActionResult> ChangeStatus(
            [FromRoute] string identifier,
            ChangeConversationStatusRequest model,
            [FromServices] ICommandBus commandBus,
            CancellationToken token)

        {
            var command = new ChangeConversationStatusCommand(identifier, model.Status);
            await commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}
