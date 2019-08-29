using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Query;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Authorization;
using Cloudents.Core.Extension;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        /// Delete answer from the system
        /// </summary>
        /// <param name="ids">a list of ids to delete</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(/*Roles = Roles.Admin*/)]
        public async Task<ActionResult> DeleteAnswerAsync([FromQuery(Name = "id")] IEnumerable<Guid> ids, CancellationToken token)
        {
            foreach (var id in ids)
            {

                var command = new DeleteAnswerCommand(id);
                await _commandBus.DispatchAsync(command, token);
            }
            return Ok();
        }

        [HttpGet("flagged")]
        public async Task<IEnumerable<FlaggedAnswerDto>> FlagAsync(CancellationToken token)
        {
         
            var query = new FlaggedAnswerQuery(User.GetCountryClaim());
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpPost("unFlag")]
        public async Task<ActionResult> UnFlagAnswerAsync([FromBody] UnFlagAnswerRequest model, CancellationToken token)
        {
            var command = new UnFlagAnswerCommand(model.Id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}
