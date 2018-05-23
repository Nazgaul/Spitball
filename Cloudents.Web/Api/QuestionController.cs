using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Policy = SignInStep.PolicyAll)]
    public class QuestionController : Controller
    {
        private readonly Lazy<ICommandBus> _commandBus;
        private readonly IMapper _mapper;
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;

        public QuestionController(Lazy<ICommandBus> commandBus, IMapper mapper,
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
            await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        [HttpGet("subject")]
        public async Task<IActionResult> GetSubjectsAsync([FromServices] IQueryBus queryBus, CancellationToken token)
        {
            var result = await queryBus.QueryAsync<IEnumerable<QuestionSubjectDto>>(token).ConfigureAwait(false);
            return Ok(result);
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

        [HttpPut("correct")]
        public async Task<IActionResult> MarkAsReadAsync(MarkAsCorrectRequest model, CancellationToken token)
        {
            var command = _mapper.Map<CreateQuestionCommand>(model);
            await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionAsync(long id, [FromServices] IQuestionRepository repository, CancellationToken token)
        {
            var retVal = await repository.GetQuestionDtoAsync(id, token).ConfigureAwait(false);
            return Ok(retVal);
        }

        [AllowAnonymous, HttpGet]
        public async Task<IActionResult> GetQuestionsAsync(string term, string[] source, [FromServices] IQuestionSearch questionSearch,
            CancellationToken token)
        {
            var retVal = await questionSearch.SearchAsync(term, source, token).ConfigureAwait(false);
            return Ok(retVal);
        }
    }
}