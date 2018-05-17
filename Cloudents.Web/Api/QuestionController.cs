using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Read;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
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
    [Authorize(Policy = SignInStep.PolicyAll)]
    public class QuestionController : Controller
    {
        private readonly ICommandBus _commandBus;
        private readonly IMapper _mapper;
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;

        public QuestionController(ICommandBus commandBus, IMapper mapper,
            IBlobProvider<QuestionAnswerContainer> blobProvider)
        {
            _commandBus = commandBus;
            _mapper = mapper;
            _blobProvider = blobProvider;
        }

        [HttpPost, ValidateModel]
        public async Task<IActionResult> CreateQuestionAsync([FromBody]QuestionRequest model, CancellationToken token)
        {
          
            var command = _mapper.Map<CreateQuestionCommand>(model);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        [HttpGet("subject")]
        public async Task<IActionResult> GetSubjectsAsync([FromServices] IQueryBus queryBus, CancellationToken token)
        {
            var result = await queryBus.QueryAsync<IEnumerable<QuestionSubjectDto>>(token).ConfigureAwait(false);
            return Ok(result);
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileAsync(UploadFileRequest model, CancellationToken token)
        {
            var userId = User.GetUserId();
            var fileName = $"{userId}.{Guid.NewGuid()}.{model.File.FileName}";
            await _blobProvider.UploadStreamAsync(fileName, model.File.OpenReadStream(), model.File
                    .ContentType, false, 60 * 24, token).ConfigureAwait(false);

            return Ok(new { fileName });
        }
    }
}