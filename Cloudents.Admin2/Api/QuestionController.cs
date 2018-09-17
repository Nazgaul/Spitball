using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Admin2.Models;
using Cloudents.Core;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Query.Admin;
using Cloudents.Core.Storage;
using Cloudents.Core.Storage.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController/*, Authorize*/]
    public class QuestionController : ControllerBase
    {
        private readonly Lazy<ICommandBus> _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly IQueueProvider _queueProvider;

        public QuestionController(Lazy<ICommandBus> commandBus, IQueryBus queryBus, IQueueProvider queueProvider)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
            _queueProvider = queueProvider;
        }

        [HttpPost]
        public async Task<ActionResult> CreateQuestionAsync([FromBody]CreateQuestionRequest model, CancellationToken token)
        {
            var userId = await _queryBus.QueryAsync<long>(new AdminEmptyQuery(), token);
            var message = new NewQuestionMessage(model.SubjectId, model.Text, model.Price, userId);
            await _queueProvider.InsertQuestionMessageAsync(message, token);
            return Ok();
        }

        [HttpGet("subject")]
        [ResponseCache(Duration = TimeConst.Day)]
        public async Task<IEnumerable<QuestionSubjectDto>> GetSubjectsAsync([FromServices] IQueryBus queryBus, CancellationToken token)
        {
            var query = new QuestionSubjectQuery();
            var result = await queryBus.QueryAsync(query, token).ConfigureAwait(false);
            return result;
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteQuestionAsync(IEnumerable<long> ids, CancellationToken token)
        {
            foreach (var id in ids)
            {

                var command = new DeleteQuestionCommand(id);

                await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
            }
            return Ok();
        }

    }
}