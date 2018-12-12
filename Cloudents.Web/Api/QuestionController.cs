using Cloudents.Core;
using Cloudents.Core.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Cloudents.Core.Questions.Commands.FlagQuestion;
using Cloudents.Core.Votes.Commands.AddVoteQuestion;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly ICommandBus _commandBus;

        private readonly IProfileUpdater _profileUpdater;
        private readonly UserManager<RegularUser> _userManager;
        private readonly IStringLocalizer<QuestionController> _localizer;
        private readonly IQuestionSearch _questionSearch;

        public QuestionController(ICommandBus commandBus, UserManager<RegularUser> userManager,
            IStringLocalizer<QuestionController> localizer, IQuestionSearch questionSearch,
            IProfileUpdater profileUpdater)
        {
            _commandBus = commandBus;
            _userManager = userManager;
            _localizer = localizer;
            _questionSearch = questionSearch;
            _profileUpdater = profileUpdater;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CreateQuestionResponse>> CreateQuestionAsync([FromBody]CreateQuestionRequest model,
            [ClaimModelBinder(AppClaimsPrincipalFactory.Score)] int score,
            CancellationToken token)
        {

            Debug.Assert(model.SubjectId != null, "model.SubjectId != null");
            var toasterMessage = _localizer["PostedQuestionToasterOk"];
            try
            {
                var command = new CreateQuestionCommand(model.SubjectId.Value, model.Text, model.Price,
                    _userManager.GetLongUserId(User), model.Files, model.Color.GetValueOrDefault());
                await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            }
            catch (DuplicateRowException)
            {
                toasterMessage = _localizer["PostedQuestionToasterPending"];
            }
            catch (InsufficientFundException)
            {
                ModelState.AddModelError(string.Empty, _localizer["InSufficientFunds"]);
                return BadRequest(ModelState);
            }
            catch (QuotaExceededException)
            {
                ModelState.AddModelError(string.Empty, _localizer["QuestionFlood"]);
                return BadRequest(ModelState);
            }
            if (score < Privileges.Post)
            {
                toasterMessage = _localizer["PostedQuestionToasterPending"];
            }

            return new CreateQuestionResponse(toasterMessage);


        }

        [HttpGet("subject")]
        [ResponseCache(Duration = TimeConst.Day, VaryByQueryKeys = new[] { "*" })]
        public IEnumerable<QuestionSubjectResponse> GetSubjectsAsync()
        {
            var values = QuestionSubjectMethod.GetValues();
            return values.Select(s => new QuestionSubjectResponse((int)s, s.GetEnumLocalization()));
        }

        [HttpPut("correct")]
        public async Task<IActionResult> MarkAsCorrectAsync([FromBody]MarkAsCorrectRequest model, CancellationToken token)
        {
            // var link = Url.Link("WalletRoute", null);
            var command = new MarkAnswerAsCorrectCommand(model.AnswerId, _userManager.GetLongUserId(User));

            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<QuestionDetailDto>> GetQuestionAsync(long id,
            [FromServices] IQueryBus bus, CancellationToken token)
        {
            var retValTask =  bus.QueryAsync(new QuestionDataByIdQuery(id), token);
            var votesTask = Task.FromResult<Dictionary<Guid?, VoteType>>(null);

            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetLongUserId(User);
                var queryTags = new UserVotesQuestionQuery(userId, id);
                votesTask = bus.QueryAsync<IEnumerable<UserVoteAnswerDto>>(queryTags, token)
                    .ContinueWith(
                        t2 =>
                        {
                            return t2.Result.ToDictionary(x => x.Id, s => s.Vote);
                        }, token);

            }

            await Task.WhenAll(retValTask, votesTask);
            var retVal = retValTask.Result;
            if (retVal == null)
            {
                return NotFound();
            }

            if (votesTask.Result == null)
            {
                return retVal;
            }

            if (votesTask.Result.TryGetValue(null, out var p))
            {
                retVal.Vote.Vote = p;
            }

            foreach (var answer in retVal.Answers)
            {
                if (votesTask.Result.TryGetValue(answer.Id, out var p2))
                {
                    retVal.Vote.Vote = p2;
                }
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
                var command = new DeleteQuestionCommand(model.Id, _userManager.GetLongUserId(User));
                await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
                return Ok();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous, HttpGet(Name = "QuestionSearch")]
        public async Task<ActionResult<WebResponseWithFacet<QuestionFeedDto>>> GetQuestionsAsync(
            [FromQuery]QuestionsRequest model,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            [FromServices] IQueryBus queryBus,
           CancellationToken token)
        {
            var query = new QuestionsQuery(model.Term, model.Source,
                model.Page.GetValueOrDefault(),
                model.Filter?.Where(w => w.HasValue).Select(s => s.Value),
                profile.Country);


            var queueTask = _profileUpdater.AddTagToUser(model.Term, User, token);

            var votesTask = Task.FromResult<Dictionary<long, VoteType>>(null);

            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetLongUserId(User);
                var queryTags = new UserVotesByCategoryQuery(userId);
                votesTask = queryBus.QueryAsync<IEnumerable<UserVoteQuestionDto>>(queryTags, token)
                    .ContinueWith(
                        t2 =>
                        {
                            return t2.Result.ToDictionary(x => x.Id, s => s.Vote);
                        }, token);

            }

            var taskResult = _questionSearch.SearchAsync(query, token);
            await Task.WhenAll(votesTask, taskResult);

            var result = taskResult.Result;
            string nextPageLink = null;
            if (result.Result.Count > 0)
            {
                nextPageLink = Url.NextPageLink("QuestionSearch", null, model);
            }

            await queueTask;
            return new WebResponseWithFacet<QuestionFeedDto>
            {
                Result = result.Result.Select(s =>
                {
                    if (votesTask != null && votesTask.Result.TryGetValue(s.Id, out var param))
                    {
                        s.Vote.Vote = param;
                    }

                    return s;
                }),
                Filters = new IFilters[]
                {
                    new Filters<string>(nameof(QuestionsRequest.Filter),_localizer["FilterTypeTitle"],
                        result.FacetState.Select(s=> new KeyValuePair<string, string>(s.ToString("G"),s.GetEnumLocalization()))),

                    new Filters<string>(nameof(QuestionsRequest.Source),_localizer["SubjectTypeTitle"],
                        QuestionSubjectMethod.GetValues(result.FacetSubject)
                            .Select(s => new KeyValuePair<string, string>(s.ToString("G"), s.GetEnumLocalization())))
                },
                NextPageLink = nextPageLink
            };
        }


        [HttpPost("vote")]
        public async Task<IActionResult> VoteAsync([FromBody] AddVoteQuestionRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new AddVoteQuestionCommand(userId, model.Id, model.VoteType);

            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPost("flag")]
        public async Task<IActionResult> FlagAsync([FromBody] FlagQuestionRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new FlagQuestionCommand(userId, model.Id, model.FlagReason);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

    }
}