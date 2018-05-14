using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class QuestionController : Controller
    {
        private readonly ICommandBus _commandBus;

        public QuestionController(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        [HttpPost, Authorize(Policy = SignInStep.PolicyAll)]
        [ValidateModel]
        public async Task<IActionResult> CreateQuestionAsync(QuestionRequest model, CancellationToken token)
        {
            //await _commandBus.DispatchAsync(model, token).ConfigureAwait(false);
            return Ok();
        }
    }
}