using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class AnswerController : Controller
    {
        private readonly ICommandBus _commandBus;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public AnswerController(ICommandBus commandBus, IMapper mapper, UserManager<User> userManager)
        {
            _commandBus = commandBus;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost, ValidateModel]
        public async Task<IActionResult> CreateAnswerAsync([FromBody]CreateAnswerRequest model,
            [FromServices] IQueryBus queryBus,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var link = Url.Link("QuestionRoute", new { id = model.QuestionId });
            var command = new CreateAnswerCommand(model.QuestionId, model.Text, userId, model.Files, link);
            var t1 = _commandBus.DispatchAsync(command, token);

            var query = new NextQuestionQuery(model.QuestionId, userId);
            var t2 = queryBus.QueryAsync(query, token);
            await Task.WhenAll(t1, t2).ConfigureAwait(false);

            return Ok(new
            {
                nextQuestions = t2.Result
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswerAsync(DeleteAnswerRequest model, CancellationToken token)
        {
            try
            {
                var command = _mapper.Map<DeleteAnswerCommand>(model);
                await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
                return Ok();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }
    }
}