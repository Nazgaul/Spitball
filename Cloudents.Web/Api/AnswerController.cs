using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]
    public class AnswerController : ControllerBase
    {
        internal const string CreateAnswerPurpose = "CreateAnswer";
        private readonly ICommandBus _commandBus;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ITimeLimitedDataProtector _dataProtector;

        public AnswerController(ICommandBus commandBus, IMapper mapper, UserManager<User> userManager, IDataProtectionProvider xxx)
        {
            _commandBus = commandBus;
            _mapper = mapper;
            _userManager = userManager;
            _dataProtector = xxx.CreateProtector(CreateAnswerPurpose).ToTimeLimitedDataProtector();
        }

        [HttpPost]
        public async Task<CreateAnswerResponse> CreateAnswerAsync([FromBody]CreateAnswerRequest model,
            [FromServices] IQueryBus queryBus,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
           // var code = _dataProtector.Protect(userId.ToString(), DateTimeOffset.UtcNow.AddDays(2));
            var link = Url.Action("Index", "Question", new { id = model.QuestionId });
            var command = new CreateAnswerCommand(model.QuestionId, model.Text, userId, model.Files, link);
            var t1 = _commandBus.DispatchAsync(command, token);

            var query = new NextQuestionQuery(model.QuestionId, userId);
            var t2 = queryBus.QueryAsync(query, token);
            await Task.WhenAll(t1, t2).ConfigureAwait(false);

            return new CreateAnswerResponse
            {
                NextQuestions = t2.Result
            };
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]

        public async Task<IActionResult> DeleteAnswerAsync([FromRoute]DeleteAnswerRequest model, CancellationToken token)
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