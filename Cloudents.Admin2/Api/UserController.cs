using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Admin2.Models;
using Cloudents.Core.Command;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]"), ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public UserController(ICommandBus commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        // GET
        [HttpPost("sendTokens")]
        public async Task<IActionResult> Post(SendTokenRequest model, CancellationToken token)
        {
            var command = new DistributeTokensCommand(model.UserId, model.Tokens, ActionType.None);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpGet("cashOut")]
        public async Task<IEnumerable<CashOutDto>> Get(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            return await _queryBus.QueryAsync<IEnumerable<CashOutDto>>(query, token);
        }
    }
}