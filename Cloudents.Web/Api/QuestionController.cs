using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Item.Commands.FlagItem;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Query.Questions;
using Cloudents.Core.DTOs.Questions;
using Microsoft.AspNetCore.SignalR;
using Cloudents.Web.Hubs;
using Cloudents.Core;

namespace Cloudents.Web.Api
{

    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class QuestionController : ControllerBase
    {
        private readonly ICommandBus _commandBus;

        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<QuestionController> _localizer;

        public QuestionController(ICommandBus commandBus, UserManager<User> userManager,
            IStringLocalizer<QuestionController> localizer
           )
        {
            _commandBus = commandBus;
            _userManager = userManager;
            _localizer = localizer;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CreateQuestionAsync([FromBody]CreateQuestionRequest model,
            [FromServices] IHubContext<SbHub> hubContext,
            CancellationToken token)
        {
         
            var userId = _userManager.GetLongUserId(User);

            var toasterMessage = _localizer["PostedQuestionToasterOk"];
            try
            {
                var command = new CreateQuestionCommand(model.Text,
                    userId, model.Course);
                await _commandBus.DispatchAsync(command, token);
            }
            catch (DuplicateRowException)
            {
                toasterMessage = _localizer["PostedQuestionToasterDuplicate"];
            }
            catch (SqlConstraintViolationException)
            {
                toasterMessage = _localizer["PostedQuestionToasterCourseNotExists"];
                //Console.WriteLine(e);
                //throw;
            }

            await hubContext.Clients.User(userId.ToString()).SendCoreAsync("Message", new object[]
            {
                new SignalRTransportType(SignalRType.System, SignalREventAction.Toaster, new
                    {
                        text = toasterMessage.Value
                    }
                )}, token);
            return Ok();
        }





        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<QuestionDetailDto>> GetQuestionAsync(long id,
            [FromServices] IQueryBus bus,
            [FromServices] IUrlBuilder urlBuilder, CancellationToken token)
        {
            var query = new QuestionDataByIdQuery(id);
            var retVal = await  bus.QueryAsync(query, token);
            if (retVal == null)
            {
                return NotFound();
            }
            retVal.User.Image = urlBuilder.BuildUserImageEndpoint(retVal.User.Id, retVal.User.Image);

            foreach (var answer in retVal.Answers)
            {
                answer.User.Image = urlBuilder.BuildUserImageEndpoint(answer.User.Id, answer.User.Image);
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
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }


        [HttpPost("flag")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> FlagAsync([FromBody] FlagQuestionRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new FlagQuestionCommand(userId, model.Id, model.FlagReason);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
            
        }


    }
}