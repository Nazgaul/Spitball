using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Item.Commands.FlagItem;
using Cloudents.Command.Votes.Commands.AddVoteAnswer;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Resources;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AnswerController : ControllerBase
    {
        //internal const string CreateAnswerPurpose = "CreateAnswer";
        private readonly ICommandBus _commandBus;
        private readonly IStringLocalizer<AnswerController> _localizer;
        private readonly UserManager<RegularUser> _userManager;

        public AnswerController(ICommandBus commandBus, UserManager<RegularUser> userManager, IStringLocalizer<AnswerController> localizer)
        {
            _commandBus = commandBus;
            _userManager = userManager;
            _localizer = localizer;
        }

        [HttpPost]
        public async Task<ActionResult<CreateAnswerResponse>> CreateAnswerAsync([FromBody]CreateAnswerRequest model,
            [FromServices] IQueryBus queryBus,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            try
            {
                var command = new CreateAnswerCommand(model.QuestionId, model.Text, userId, model.Files);
                var t1 = _commandBus.DispatchAsync(command, token);

                var query = new NextQuestionQuery(model.QuestionId, userId);
                var t2 = queryBus.QueryAsync(query, token);

                await Task.WhenAll(t1, t2);

             

                return new CreateAnswerResponse
                {
                    NextQuestions = t2.Result
                };
            }
            catch (QuotaExceededException)
            {
                ModelState.AddModelError(nameof(model.Text), _localizer["You exceed your quota of answers"]);
                return BadRequest(ModelState);
            }
            catch (MoreThenOneAnswerException)
            {
                ModelState.AddModelError(nameof(model.Text), _localizer["More then one answer"]);
                return BadRequest(ModelState);
            }
            catch (QuestionAlreadyAnsweredException)
            {
                ModelState.AddModelError(nameof(model.Text), _localizer["This question have correct answer"]);
                return BadRequest(ModelState);
            }
            catch (DuplicateRowException)
            {
                ModelState.AddModelError(nameof(model.Text), _localizer["DuplicateAnswer"]);
                return BadRequest(ModelState);
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError(nameof(model.Text), _localizer["QuestionNotExists"]);
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]

        public async Task<IActionResult> DeleteAnswerAsync([FromRoute]DeleteAnswerRequest model, CancellationToken token)
        {
            try
            {
                var command = new DeleteAnswerCommand(model.Id, _userManager.GetLongUserId(User));
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError(nameof(model.Id), _localizer["Answer does not exists"]);
                return BadRequest(ModelState);
            }
        }


        [HttpPost("vote")]
        public async Task<IActionResult> VoteAsync(
            [FromBody] AddVoteAnswerRequest model,
            [FromServices] IStringLocalizer<SharedResource> resource,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            try
            {
                var command = new AddVoteAnswerCommand(userId, model.Id, model.VoteType);
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (NoEnoughScoreException)
            {
                string voteMessage = resource[$"{model.VoteType:G}VoteError"];
                ModelState.AddModelError(nameof(AddVoteDocumentRequest.Id), voteMessage);
                return BadRequest(ModelState);
            }
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError(nameof(AddVoteDocumentRequest.Id), _localizer["VoteCantVote"]);
                return BadRequest(ModelState);
            }

            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("flag")]
        public async Task<IActionResult> FlagAsync([FromBody] FlagAnswerRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            try
            {
                var command = new FlagAnswerCommand(userId, model.Id, model.FlagReason);
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (NoEnoughScoreException)
            {
                ModelState.AddModelError(nameof(AddVoteDocumentRequest.Id), _localizer["VoteNotEnoughScore"]);
                return BadRequest(ModelState);
            }
        }
    }
}