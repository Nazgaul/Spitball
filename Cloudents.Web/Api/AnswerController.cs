using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;

        public AnswerController(ICommandBus commandBus, IMapper mapper, IBlobProvider<QuestionAnswerContainer> blobProvider)
        {
            _commandBus = commandBus;
            _mapper = mapper;
            _blobProvider = blobProvider;
        }

        [HttpPost,ValidateModel]
        public async Task<IActionResult> CreateAnswerAsync([FromBody]CreateAnswerRequest model, CancellationToken token)
        {
            var command = _mapper.Map<CreateAnswerCommand>(model);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }


        [HttpPost, ValidateModel]
        public async Task<IActionResult> UpVoteAsync([FromBody]UpVoteAnswerRequest model, CancellationToken token)
        {
            var command = _mapper.Map<UpVoteAnswerCommand>(model);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileAsync(UploadFileRequest model,
            [FromServices] UserManager<User> userManager,
            CancellationToken token)
        {
            var userId = userManager.GetUserId(User);
            var fileName = $"{userId}.{Guid.NewGuid()}.{model.File.FileName}";
            await _blobProvider.UploadStreamAsync(fileName, model.File.OpenReadStream(), model.File
                    .ContentType, false, 60 * 24, token).ConfigureAwait(false);

            return Ok(new { fileName });
        }
    }
}