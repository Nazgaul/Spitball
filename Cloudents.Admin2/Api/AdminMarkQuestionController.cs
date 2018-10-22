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
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController/*,Authorize*/]
    public class AdminMarkQuestionController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public AdminMarkQuestionController(ICommandBus commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        /// <summary>
        /// Get a list of question without correct answer
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<QuestionWithoutCorrectAnswerDto>> Get(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            var t =  await _queryBus.QueryAsync<IEnumerable<QuestionWithoutCorrectAnswerDto>>(query, token);
            return t.Take(100);
        }

        /// <summary>
        /// Set a question as correct
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
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
