using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly Lazy<ICommandBus> _commandBus;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public QuestionController(Lazy<ICommandBus> commandBus, IMapper mapper, UserManager<User> userManager)
        {
            _commandBus = commandBus;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateQuestionAsync([FromBody]CreateQuestionRequest model, CancellationToken token)
        {
            try
            {
                var command = _mapper.Map<CreateQuestionCommand>(model);
                await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);

                return CreatedAtAction(nameof(GetQuestionAsync), new {id = command.Id});
            }
            catch (InvalidOperationException)
            {
                ModelState.AddModelError(string.Empty,"You need to wait before asking a new question");
                return BadRequest(ModelState);
            }
        }

        [HttpGet("subject")]
        [ResponseCache(Duration = TimeConst.Day)]
        public async Task<IEnumerable<QuestionSubjectDto>> GetSubjectsAsync([FromServices] IQueryBus queryBus, CancellationToken token)
        {
            var query = new QuestionSubjectQuery();
            return await queryBus.QueryAsync(query, token).ConfigureAwait(false);
        }

        [HttpPut("correct")]
        public async Task<IActionResult> MarkAsCorrectAsync([FromBody]MarkAsCorrectRequest model, CancellationToken token)
        {
            // var link = Url.Link("WalletRoute", null);
            var command = new MarkAnswerAsCorrectCommand(model.AnswerId, _userManager.GetLongUserId(User));

            await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<QuestionDetailDto>> GetQuestionAsync(long id,
            [FromServices] IQueryBus bus, CancellationToken token)
        {
            var retVal = await bus.QueryAsync(new QuestionDataByIdQuery(id), token).ConfigureAwait(false);
            if (retVal == null)
            {
                return NotFound();
            }
            return retVal;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteQuestionAsync([FromRoute]DeleteQuestionRequest model, CancellationToken token)
        {
            try
            {
                var command = _mapper.Map<DeleteQuestionCommand>(model);
                await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
                return Ok();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous, HttpGet(Name = "QuestionSearch")]
        public async Task<ActionResult<WebResponseWithFacet2<QuestionDto>>> GetQuestionsAsync([FromQuery]GetQuestionsRequest model,
            [FromServices] IQueryBus queryBus,
            CancellationToken token)
        {
            var query = _mapper.Map<QuestionsQuery>(model);
            var result = await queryBus.QueryAsync(query, token).ConfigureAwait(false);
            var p = result.Result?.ToList();
            string nextPageLink = null;
            if (p?.Any() == true)
            {
                nextPageLink = Url.NextPageLink("QuestionSearch", null, model);
            }

            return new WebResponseWithFacet2<QuestionDto>
            {
                Result = p,
                Filters = new Dictionary<string, IEnumerable<string>>
                {
                    ["Content"] = result.Facet,
                    ["State"] = new []
                    {
                        "Unanswered",
                        "Answered",
                        "Sold"
                    }
                } ,
                NextPageLink = nextPageLink
            };
        }
    }
}