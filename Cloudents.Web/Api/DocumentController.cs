using Autofac.Features.Indexed;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Documents.ChangePrice;
using Cloudents.Command.Documents.Delete;
using Cloudents.Command.Documents.PurchaseDocument;
using Cloudents.Command.Item.Commands.FlagItem;
using Cloudents.Command.Votes.Commands.AddVoteDocument;
using Cloudents.Core.DTOs.Documents;
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wangkanai.Detection;

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
        private readonly IDocumentDirectoryBlobProvider _blobProvider;
        private readonly IStringLocalizer<DocumentController> _localizer;

        private static readonly Task<string> Task = System.Threading.Tasks.Task.FromResult<string>(null);

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
            _blobProvider = blobProvider;
            _localizer = localizer;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<DocumentPreviewResponse>> GetAsync(long id,
            [FromServices] IQueueProvider queueProvider,
            [FromServices] ICrawlerResolver crawlerResolver,
            [FromServices] IIndex<DocumentType, IDocumentGenerator> generatorIndex,
            [FromServices] IUrlBuilder urlBuilder,
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
            
            model.Document.User.Image = urlBuilder.BuildUserImageEndpoint(model.Document.User.Id, model.Document.User.Image);
            if (model.Tutor != null)
            {
                model.Tutor.Image =
                    urlBuilder.BuildUserImageEndpoint(model.Tutor.UserId, model.Tutor.Image);
            }

            var tQueue = queueProvider.InsertMessageAsync(new UpdateDocumentNumberOfViews(id), token);
            var textTask = Task;
            if (crawlerResolver.Crawler != null && model.Document.DocumentType == DocumentType.Document )
            {
                textTask = _blobProvider.DownloadTextAsync("text.txt", query.Id.ToString(), token);
            }

            var files = await generatorIndex[model.Document.DocumentType].GeneratePreviewAsync(model, userId.GetValueOrDefault(-1), token);
            await System.Threading.Tasks.Task.WhenAll(tQueue, textTask);
            model.Document.Url = Url.DocumentUrl(model.Document.Id, model.Document.Title);
            return new DocumentPreviewResponse(model, files, textTask.Result);
        }

        [HttpPost, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CreateDocumentAsync([FromBody]CreateDocumentRequest model,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            if (!model.BlobName.StartsWith("file-", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.Name), "Invalid file name");
                return BadRequest(ModelState);
            }
            var command = new CreateDocumentCommand(model.BlobName, model.Name,
                model.Course, userId, model.Price, model.Description);
            await _commandBus.DispatchAsync(command, token);

            //var url = Url.RouteUrl("ShortDocumentLink", new
            //{
            //    base62 = new Base62(command.Id).ToString()
            //});
            return Ok();
        }





        [HttpPost("vote"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> VoteAsync([FromBody]
            AddVoteDocumentRequest model,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            try
            {
                var command = new AddVoteDocumentCommand(userId, model.Id, model.VoteType);
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (DuplicateRowException)
            {
                ModelState.AddModelError(nameof(AddVoteDocumentRequest.Id), "Cannot vote twice");
                return BadRequest(ModelState);
            }
          
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError(nameof(AddVoteDocumentRequest.Id), _localizer["VoteCantVote"]);
                return BadRequest(ModelState);
            }

            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("flag"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> FlagAsync([FromBody] FlagDocumentRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            try
            {
                var command = new FlagDocumentCommand(userId, model.Id, model.FlagReason);
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (NoEnoughScoreException)
            {
                ModelState.AddModelError(nameof(AddVoteDocumentRequest.Id), _localizer["VoteNotEnoughScore"]);
                return BadRequest(ModelState);
            }
        }


        [HttpPost("purchase"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PurchaseAsync([FromBody] PurchaseDocumentRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            try
            {
                var command = new PurchaseDocumentCommand(model.Id, userId);
                await _commandBus.DispatchAsync(command, token);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (InsufficientFundException)
            {
                ModelState.AddModelError(string.Empty, _localizer["InSufficientFunds"]);
                return BadRequest(ModelState);
            }
            catch (DuplicateRowException)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("price"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ChangePriceAsync([FromBody] ChangePriceRequest model, CancellationToken token)
        {
            //if (model.Price < 0)
            //{
            //    ModelState.AddModelError(string.Empty, _localizer["PriceNeedToBeGreaterOrEqualZero"]);
            //    return BadRequest(ModelState);
            //}
            var userId = _userManager.GetLongUserId(User);
            var command = new ChangeDocumentPriceCommand(model.Id, userId, model.Price);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpDelete("{id}"), Authorize]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteDocumentAsync([FromRoute]DeleteDocumentRequest model, CancellationToken token)
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


        [HttpGet("similar")]
        public async Task<IEnumerable<DocumentFeedDto>> GetSimilarDocumentsAsync(
            [FromQuery] SimilarDocumentsRequest request,
            [FromServices] ICrawlerResolver crawlerResolver,
             CancellationToken token)
        {
            if (crawlerResolver.Crawler != null)
            {
                return Enumerable.Empty<DocumentFeedDto>();
            }
            var query = new SimilarDocumentsQuery(request.DocumentId);
            var res = await _queryBus.QueryAsync(query, token);
            return res.Select(s =>
            {
                s.Url = Url.DocumentUrl(s.Id, s.Title);
                return s;
            });
        }

        [HttpPost("rename"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RenameDocumentAsync([FromBody] RenameDocumentRequest model,
                CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new RenameDocumentCommand(userId, model.DocumentId, model.Name);
            try
            {
                await _commandBus.DispatchAsync(command, token);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            return Ok();
        }

        [NonAction]
        public override Task FinishUploadAsync(UploadRequestFinish model, string blobName, CancellationToken token)
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }

    }
}