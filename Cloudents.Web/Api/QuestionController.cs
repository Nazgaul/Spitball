using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Policy = SignInStep.Finish)]
    public class QuestionController : Controller
    {
        private readonly Lazy<ICommandBus> _commandBus;
        private readonly IMapper _mapper;

        public QuestionController(Lazy<ICommandBus> commandBus, IMapper mapper)
        {
            _commandBus = commandBus;
            _mapper = mapper;
        }

        [HttpPost, ValidateModel]
        public async Task<IActionResult> CreateQuestionAsync([FromBody]QuestionRequest model, CancellationToken token)
        {
            var command = _mapper.Map<CreateQuestionCommand>(model);
            await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        [HttpGet("subject")]
        public async Task<IActionResult> GetSubjectsAsync([FromServices] IQueryBus queryBus, CancellationToken token)
        {
            var result = await queryBus.QueryAsync<IEnumerable<QuestionSubjectDto>>(token).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPut("correct"), ValidateModel]
        public async Task<IActionResult> MarkAsReadAsync([FromBody]MarkAsCorrectRequest model, CancellationToken token)
        {
            var command = _mapper.Map<MarkAnswerAsCorrectCommand>(model);
            await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionAsync(long id,
            [FromServices] IQueryBus bus, CancellationToken token)
        {
            var retVal = await bus.QueryAsync<long, QuestionDetailDto>(id, token).ConfigureAwait(false);
            return Ok(retVal);
        }

        [AllowAnonymous, HttpGet]
        public async Task<IActionResult> GetQuestionsAsync(QuestionsRequest model,
            [FromServices] IQuestionSearch questionSearch,
            [FromServices] IQueryBus queryBus,
            CancellationToken token)
        {
            //var str = string.Join(" ", model.Term ?? Enumerable.Empty<string>());
            var query = _mapper.Map<QuestionsQuery>(model);
            if (string.IsNullOrWhiteSpace(query.Term))
            {
                var retVal = await queryBus.QueryAsync<QuestionsQuery, IEnumerable<QuestionDto>>(query, token).ConfigureAwait(false);
                return Ok(retVal);
            }
            else
            {
                var retVal = await questionSearch.SearchAsync(query, token).ConfigureAwait(false);
                return Ok(retVal);
            }
        }
    }
}