﻿using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Query;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminTutorController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public AdminTutorController(ICommandBus commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        [HttpGet]
        public async Task<IEnumerable<PendingTutorsDto>> GetPendingTutorsAsync(CancellationToken token)
        {
            var query = new AdminPendingTutorsQuery();
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpPost("approve")]
        public async Task<IActionResult> ApproveTutor([FromBody] ApproveTutorRequest model,
        CancellationToken token)
        {
            var command = new ApproveTutorCommand(model.Id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutor(long id,
                CancellationToken token)
        {
            var command = new DeleteTutorCommand(id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}
