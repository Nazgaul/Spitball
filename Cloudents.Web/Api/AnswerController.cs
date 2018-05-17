using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IMapper = AutoMapper.IMapper;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Policy = SignInStep.PolicyAll)]
    public class AnswerController : Controller
    {
        private readonly ICommandBus _commandBus;
        private readonly IMapper _mapper;

        public AnswerController(ICommandBus commandBus, IMapper mapper)
        {
            _commandBus = commandBus;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnswerAsync(CreateAnswerRequest model, CancellationToken token)
        {
            var command = _mapper.Map<CreateAnswerCommand>(model);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }
    }
}