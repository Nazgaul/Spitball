using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
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
        public async Task<IActionResult> CreateQuestionAsync([FromBody]CreateQuestionRequest model, CancellationToken token)
        {
            var command = _mapper.Map<CreateQuestionCommand>(model);
            await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        [HttpGet("subject")]
        [ResponseCache(Duration = TimeConst.Day)]
        public async Task<IActionResult> GetSubjectsAsync([FromServices] IQueryBus queryBus, CancellationToken token)
        {
            var query = new QuestionSubjectQuery();
            var result = await queryBus.QueryAsync<IEnumerable<QuestionSubjectDto>>(query, token).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPut("correct"), ValidateModel]
        public async Task<IActionResult> MarkAsCorrectAsync([FromBody]MarkAsCorrectRequest model, CancellationToken token)
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
            var retVal = await bus.QueryAsync<QuestionDetailDto>(new QuestionDetailQuery(id), token).ConfigureAwait(false);
            if (retVal == null)
            {
                return NotFound();
            }
            return Ok(retVal);
        }

        [HttpDelete("{id}"), ValidateModel]
        public async Task<IActionResult> DeleteQuestionAsync(DeleteQuestionRequest model, CancellationToken token)
        {
            var command = _mapper.Map<DeleteQuestionCommand>(model);
            await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        [AllowAnonymous, HttpGet(Name = "QuestionSearch")]
        public async Task<IActionResult> GetQuestionsAsync(GetQuestionsRequest model,
            [FromServices] IQueryBus queryBus,
            CancellationToken token)
        {
            var query = _mapper.Map<QuestionsQuery>(model);
            //if (string.IsNullOrWhiteSpace(query.Term))
            //{
            var result = await queryBus.QueryAsync<ResultWithFacetDto<QuestionDto>>(query, token).ConfigureAwait(false);
            //}
            //else
            //{
            //result = await questionSearch.SearchAsync(query, token).ConfigureAwait(false);
            //}
            var p = result.Result?.ToList();
            string nextPageLink = null;
            if (p?.Any() == true)
            {
                nextPageLink = Url.NextPageLink("QuestionSearch", null, model);
            }

            return Ok(new WebResponseWithFacet<QuestionDto>
            {
                Result = p,
                Facet = result.Facet,
                NextPageLink = nextPageLink
            });
        }
    }
}