using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]
    public class AnswerController : ControllerBase
    {
        //internal const string CreateAnswerPurpose = "CreateAnswer";
        private readonly ICommandBus _commandBus;
        private readonly IStringLocalizer<AnswerController> _localizer;
        private readonly UserManager<User> _userManager;

        public AnswerController(ICommandBus commandBus, UserManager<User> userManager, IStringLocalizer<AnswerController> localizer)
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
                await Task.WhenAll(t1, t2).ConfigureAwait(false);

                return new CreateAnswerResponse
                {
                    NextQuestions = t2.Result
                };
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
                await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
                return Ok();
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError(nameof(model.Id), _localizer["Answer does not exists"]);
                return BadRequest(ModelState);
            }
        }
        
    }
}