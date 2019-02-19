using Cloudents.Admin2.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Query.Admin;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminQuestionController : ControllerBase
    {
        private readonly Lazy<ICommandBus> _commandBus;
        private readonly IQueryBus _queryBus;

        public AdminQuestionController(Lazy<ICommandBus> commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        /// <summary>
        /// create a question for fictive user.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateQuestionAsync([FromBody]CreateQuestionRequest model, CancellationToken token)
        {

            var command = new CreateQuestionCommand(model.Course, model.University, model.Text, model.Price, model.Files, model.Country.ToString("G"));
            await _commandBus.Value.DispatchAsync(command, token);
            return Ok();
        }

        /// <summary>
        /// Get a list of question subject for ui
        /// </summary>
        /// <returns></returns>
        [HttpGet("subject")]
        [ResponseCache(Duration = TimeConst.Day)]
        public IEnumerable<QuestionSubjectResponse> GetSubjectsAsync()
        {
            var values = QuestionSubjectMethod.GetValues();

            return values.Select(s => new QuestionSubjectResponse((int)s, s.ToString("G")));
        }

        /// <summary>
        /// Delete question from the system
        /// </summary>
        /// <param name="ids">a list of ids to delete</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult> DeleteQuestionAsync([FromQuery(Name = "id")]IEnumerable<long> ids, CancellationToken token)
        {
            foreach (var id in ids)
            {

                var command = new DeleteQuestionCommand(id);

                await _commandBus.Value.DispatchAsync(command, token);
            }
            return Ok();
        }

        [HttpPost("approve")]
        public async Task<ActionResult> ApproveQuestionAsync([FromBody] ApproveQuestionRequest model, CancellationToken token)
        {
         

            var command = new ApproveQuestionCommand(model.Id);
            await _commandBus.Value.DispatchAsync(command, token);
          
            return Ok();
        }

        /// <summary>
        /// Get a list of question with pending state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("Pending")]
        public async Task<IEnumerable<PendingQuestionDto>> Get(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            return await _queryBus.QueryAsync<IEnumerable<PendingQuestionDto>>(query, token);
        }

        [HttpPost("upload")]
        public async Task<UploadAskFileResponse> UploadFileAsync([FromForm] UploadAskFileRequest model,
            [FromServices] IBlobProvider<QuestionAnswerContainer> blobProvider,
            CancellationToken token)
        {
            string[] supportedImages = { ".jpg", ".png", ".gif", ".jpeg", ".bmp" };

            var formFile = model.File;
            //foreach (var formFile in model.File)
            //{
            if (!formFile.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("not an image");
            }

            var extension = Path.GetExtension(formFile.FileName);

            if (!supportedImages.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException("not an image");
            }

            using (var sr = formFile.OpenReadStream())
            {
                //Image.FromStream(sr);
                var fileName = $"admin.{Guid.NewGuid()}.{formFile.FileName}";
                await blobProvider
                    .UploadStreamAsync(fileName, sr, formFile.ContentType, false, 60 * 24, token);

                return new UploadAskFileResponse(fileName);
            }

        }

        [HttpGet("flagged")]
        public async Task<IEnumerable<FlaggedQuestionDto>> FlagAsync(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            return await _queryBus.QueryAsync<IEnumerable<FlaggedQuestionDto>>(query, token);
        }

        [HttpPost("unFlag")]
        public async Task<ActionResult> UnFlagAnswerAsync([FromBody] UnFlagQuestionRequest model, CancellationToken token)
        {
            var command = new UnFlagQuestionCommand(model.Id);
            await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }
    }
}