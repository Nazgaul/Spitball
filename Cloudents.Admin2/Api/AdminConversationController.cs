using Cloudents.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminConversationController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public AdminConversationController(ICommandBus commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        [HttpGet]
        public async Task<IEnumerable<ConversationDto>> Conversationsync(CancellationToken token)
        {
            var query = new AdminConversationsQuery();
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("details")]
        public async Task<IEnumerable<ConversationDetailsDto>> ConversationDetailsync(
            [FromQuery(Name = "id")]Guid id,
            CancellationToken token)
        {
            var query = new AdminConversationDetailsQuery(id);
            return await _queryBus.QueryAsync(query, token);
        }


        [HttpGet("{id}")]
        public async Task<IEnumerable<ChatMessageDto>> Get(string id, 
            [FromServices] IChatDirectoryBlobProvider blobProvider,
            [FromServices] IBinarySerializer serializer,
            CancellationToken token)
        {
            //specific conversation
            int page = 0;
            var result = await _queryBus.QueryAsync(new ChatConversationByIdQuery(id, page), token);
            return result;
        }
    }
}
