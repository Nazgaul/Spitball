using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IMapper = AutoMapper.IMapper;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Policy = PolicyType.Finish)]
    public class AnswerController : Controller
    {
        private readonly ICommandBus _commandBus;
        private readonly IMapper _mapper;

        public AnswerController(ICommandBus commandBus, IMapper mapper)
        {
            _commandBus = commandBus;
            _mapper = mapper;
        }

        [HttpPost, ValidateModel]
        public async Task<IActionResult> CreateAnswerAsync([FromBody]CreateAnswerRequest model, CancellationToken token)
        {
            var command = _mapper.Map<CreateAnswerCommand>(model);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        //[HttpPost("upVote"), ValidateModel]
        //public async Task<IActionResult> UpVoteAsync([FromBody]UpVoteAnswerRequest model, CancellationToken token)
        //{
        //    var command = _mapper.Map<UpVoteAnswerCommand>(model);
        //    await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
        //    return Ok();
        //}

        
    }
}