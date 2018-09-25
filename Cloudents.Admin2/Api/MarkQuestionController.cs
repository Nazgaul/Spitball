using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Admin2.Models;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController/*,Authorize*/]
    public class MarkQuestionController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public MarkQuestionController(ICommandBus commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        // GET: api/MarkQuestion
        [HttpGet]
        public async Task<IEnumerable<QuestionWithoutCorrectAnswerDto>> Get(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            return await _queryBus.QueryAsync<IEnumerable<QuestionWithoutCorrectAnswerDto>>(query, token);
        }

        // GET: api/MarkQuestion/5
       

        // POST: api/MarkQuestion
        [HttpPost]
        public async Task Post([FromBody] MarkQuestionAsCorrectRequest model, CancellationToken token)
        {

            Debug.Assert(model.AnswerId != null, "Model.AnswerId != null");
            Debug.Assert(model.QuestionId != null, "Model.QuestionId != null");
            var command = new MarkAnswerAsCorrectCommand(model.AnswerId.Value, model.QuestionId.Value);

            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
        }

     
    }
}
