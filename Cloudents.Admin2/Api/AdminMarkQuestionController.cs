using Cloudents.Admin2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Query.Query.Admin;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
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
        /// <param name="page"></param>
        /// <param name="token"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IEnumerable<QuestionWithoutCorrectAnswerDto>> Get(int page, CancellationToken token)
        {
            var query = new AdminPageQuery(page);
            return await _queryBus.QueryAsync(query, token);
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

            var query = new QuestionDataByIdQuery(model.QuestionId.Value);
            var questionDto = await _queryBus.QueryAsync(query, token);
            var command = new MarkAnswerAsCorrectCommand(model.AnswerId.Value, questionDto.User.Id);

            await _commandBus.DispatchAsync(command, token);
        }


    }
}
