using Autofac.Features.Indexed;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Documents.Delete;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Documents;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "this is what we want")]

    public class DocumentController : UploadControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<DocumentController> _localizer;


        public DocumentController(IQueryBus queryBus,
            ICommandBus commandBus, UserManager<User> userManager,
            IDocumentDirectoryBlobProvider blobProvider,
            IStringLocalizer<DocumentController> localizer,
            ITempDataDictionaryFactory tempDataDictionaryFactory,
            IStringLocalizer<UploadControllerBase> localizer2)
        : base(blobProvider, tempDataDictionaryFactory, localizer2)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _userManager = userManager;
            _localizer = localizer;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<DocumentPreviewResponse>> GetAsync(long id,
            [FromServices] IQueueProvider queueProvider,
            [FromServices] IIndex<DocumentType, IDocumentGenerator> generatorIndex,
            CancellationToken token)
        {
            long? userId = null;

            if (User.Identity.IsAuthenticated)
            {
                userId = _userManager.GetLongUserId(User);
            }

            var query = new DocumentById(id, userId);
            var model = await _queryBus.QueryAsync(query, token);
            if (model == null)
            {
                return NotFound();
            }

            var tQueue = queueProvider.InsertMessageAsync(new UpdateDocumentNumberOfViews(id), token);
            var taskFiles = generatorIndex[model.DocumentType].GeneratePreviewAsync(model, userId.GetValueOrDefault(-1), token);
            await Task.WhenAll(tQueue, taskFiles);
            var files = await taskFiles;
            return new DocumentPreviewResponse(model, files);
        }

       

        [HttpDelete("{id}"), Authorize]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteDocumentAsync([FromRoute] DeleteDocumentRequest model, CancellationToken token)
        {
            try
            {
                var command = new DeleteDocumentCommand(model.Id, _userManager.GetLongUserId(User));
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException)
            {
                ModelState.AddModelError("error", _localizer["SomeOnePurchased"]);
                return BadRequest(ModelState);
            }
        }


        [HttpPost("dropBox"), Authorize]
        public async Task<UploadStartResponse> UploadDropBoxAsync([FromBody] DropBoxRequest model,
           [FromServices] IRestClient client,
           [FromServices] IDocumentDirectoryBlobProvider documentDirectoryBlobProvider,
           CancellationToken token)
        {
            var (stream, _) = await client.DownloadStreamAsync(model.Link, token);
            var blobName = BlobFileName(Guid.NewGuid(), model.Name);
            await documentDirectoryBlobProvider.UploadStreamAsync(blobName, stream, token: token);

            return new UploadStartResponse(blobName);
        }
       

        //[HttpPost("rename"), Authorize]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesDefaultResponseType]
        //public async Task<IActionResult> RenameDocumentAsync([FromBody] RenameDocumentRequest model,
        //        CancellationToken token)
        //{
        //    var userId = _userManager.GetLongUserId(User);
        //    var command = new RenameDocumentCommand(userId, model.DocumentId, model.Name);
        //    try
        //    {
        //        await _commandBus.DispatchAsync(command, token);
        //    }
        //    catch (ArgumentException)
        //    {
        //        return BadRequest();
        //    }
        //    return Ok();
        //}

        [NonAction]
        public override Task FinishUploadAsync(UploadRequestFinish model, string blobName, CancellationToken token)
        {
            return Task.CompletedTask;
        }

    }
}