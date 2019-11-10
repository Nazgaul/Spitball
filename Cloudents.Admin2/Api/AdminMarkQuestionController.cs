using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminMarkQuestionController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly IUrlBuilder _urlBuilder;

        public AdminMarkQuestionController(ICommandBus commandBus, IQueryBus queryBus, IUrlBuilder urlBuilder)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
            _urlBuilder = urlBuilder;
        }

        /// <summary>
        /// Get a list of question without correct answer
        /// </summary>
        /// <param name="page"></param>
        /// <param name="token"></param>
        /// <returns></returns>

        [HttpGet]
        public IEnumerable<QuestionWithoutCorrectAnswerDto> Get(int page, CancellationToken token)
        {
            return null;
            //var query = new AdminQuestionWithoutCorrectAnswerPageQuery(page, User.GetCountryClaim());
            //var result = await _queryBus.QueryAsync(query, token);

            //return result.Select(res =>
            //{
            //    res.Url = _urlBuilder.BuildQuestionEndPoint(res.Id);
            //    return res;
            //});
        }

        /// <summary>
        /// Set a question as correct
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] MarkQuestionAsCorrectRequest model, CancellationToken token)
        {
            return Ok();
            //Debug.Assert(model.QuestionId != null, "model.QuestionId != null");
            //Debug.Assert(model.AnswerId != null, "model.AnswerId != null");
            //var query = new QuestionDataByIdQuery(model.QuestionId.Value);
            //var questionDto = await _queryBus.QueryAsync(query, token);
            //var command = new MarkAnswerAsCorrectCommand(model.AnswerId.Value, questionDto.User.Id);

            //await _commandBus.DispatchAsync(command, token);
        }


    }
}
