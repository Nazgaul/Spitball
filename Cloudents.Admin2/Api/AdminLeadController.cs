﻿using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Cloudents.Query;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminLeadController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public AdminLeadController(ICommandBus commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }
        [HttpGet]
        [Authorize(Policy = Policy.IsraelUser)]
        public async Task<IEnumerable<LeadDto>> LeadAsync([FromQuery] ItemState? status, CancellationToken token)
        {
            var query = new AdminLeadsQuery(status);
            return await _queryBus.QueryAsync(query, token);
        }
        [HttpPost("status")]
        public async Task<IActionResult> ChangeStatusAsync([FromBody] ChangeLeadStatusrRequest model, CancellationToken token)
        {
            var command = new ChangeLeadStatusCommand(model.LeadId, model.State);
            await _commandBus.DispatchAsync(command, token);
            return Ok();

        }
    }
}
