using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using System.Threading;
using Cloudents.Core.Query.Admin;
using Cloudents.Admin2.Models;
using Cloudents.Core.Command.Admin;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAnswerController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public AdminAnswerController(ICommandBus commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        /// <summary>
        /// Get a list of answers with pending state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("Pending")]
        public async Task<IEnumerable<PendingAnswerDto>> Get(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            return await _queryBus.QueryAsync<IEnumerable<PendingAnswerDto>>(query, token);
        }

        [HttpPost("approve")]
        public async Task<ActionResult> ApproveAnswerAsync([FromBody] Guid id, CancellationToken token)
        {
         
            var command = new ApproveAnswerCommand(id);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Delete answer from the system
        /// </summary>
        /// <param name="ids">a list of ids to delete</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult> DeleteAnswerAsync([FromQuery(Name = "id")] IEnumerable<Guid> ids, CancellationToken token)
        {
            foreach (var id in ids)
            {

                var command = new DeleteAnswerCommand(id);

                await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            }
            return Ok();
        }

        [HttpGet("flagged")]
        public async Task<IEnumerable<FlaggedAnswerDto>> FlagAsync(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            return await _queryBus.QueryAsync<IEnumerable<FlaggedAnswerDto>>(query, token);
        }

        [HttpPost("unFlag")]
        public async Task<ActionResult> UnFlagAnswerAsync([FromBody] Guid id, CancellationToken token)
        {
            var command = new UnFlagAnswerCommand(id);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }
    }
}
