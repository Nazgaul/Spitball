using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
    [Authorize(Policy = SignInStep.PolicyAll)]
    public class QuestionController : Controller
    {
        private readonly ICommandBus _commandBus;
        private readonly IMapper _mapper;

        public QuestionController(ICommandBus commandBus, IMapper mapper)
        {
            _commandBus = commandBus;
            _mapper = mapper;
        }

        [HttpPost, ValidateModel]
        public async Task<IActionResult> CreateQuestionAsync([FromBody]QuestionRequest model, CancellationToken token)
        {
            var command = _mapper.Map<CreateQuestionCommand>(model);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        //[HttpGet("subject")]
        //public async Task<IActionResult> GetSubjectsAsync()
        //{

        //}
    }
}