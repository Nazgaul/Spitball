using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Documents.ChangePrice;
using Cloudents.Command.Documents.Delete;
using Cloudents.Command.Documents.PurchaseDocument;
using Cloudents.Command.Item.Commands.FlagItem;
using Cloudents.Command.Votes.Commands.AddVoteDocument;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Documents;
using Cloudents.Query.Query;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Framework;
using Cloudents.Web.Models;
using Cloudents.Web.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Wangkanai.Detection;
using AppClaimsPrincipalFactory = Cloudents.Web.Identity.AppClaimsPrincipalFactory;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
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
            [FromServices] TelemetryClient telemetryClient,
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
           
            var textTask = Task;
            if (crawlerResolver.Crawler != null)
            {
                textTask = _blobProvider.DownloadTextAsync("text.txt", query.Id.ToString(), token);
            }

            var files = Enumerable.Range(0, model.Pages).Select(i =>
            {
                var uri = _blobProvider.GetPreviewImageLink(query.Id, i);
                var effect = ImageProperties.BlurEffect.None;
                if (!model.IsPurchased)
                {
                    effect = ImageProperties.BlurEffect.All;
                    if (i == 0)
                    {
                        effect = ImageProperties.BlurEffect.Part;
                    }

                }

                var properties = new ImageProperties(uri, effect);
                var url = Url.ImageUrl(properties);
                return url;
            }).ToList();


            //var filesTask = _blobProvider.FilesInDirectoryAsync("preview-", query.Id.ToString(), token).ContinueWith(
            //    result =>
            //    {
            //      return  result.Result.OrderBy(o => o, new OrderPreviewComparer()).Select((s,i) =>
            //      {
            //          var effect = ImageProperties.BlurEffect.None;
            //          if (!model.IsPurchased)
            //          {
            //              effect = ImageProperties.BlurEffect.All;
            //              if (i == 0)
            //              {
            //                  effect = ImageProperties.BlurEffect.Part;
            //              }

            //          }
            //          var properties = new ImageProperties(s, effect);
            //          var url = Url.ImageUrl(properties);
            //          return url;
            //      });
            //    }, token);

            await System.Threading.Tasks.Task.WhenAll(tQueue,  textTask);
            //var files = filesTask.Result.ToList();
            if (!files.Any())
            {
                telemetryClient.TrackTrace("Document No Preview", new Dictionary<string, string>()
                {
                    ["Id"] = id.ToString()
                });
                await queueProvider.InsertBlobReprocessAsync(id);
            }
            return new DocumentPreviewResponse(model, files, textTask.Result);
        }

        [HttpPost,Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CreateDocumentResponse>> CreateDocumentAsync([FromBody]CreateDocumentRequest model,
            [ClaimModelBinder(AppClaimsPrincipalFactory.Score)] int score,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            if (!model.BlobName.StartsWith("file-", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.Name), "Invalid file name");
                return BadRequest(ModelState);
            }
            var command = new CreateDocumentCommand(model.BlobName, model.Name, model.Type ?? "Document",
                model.Course, model.Tags, userId, model.Professor, model.Price);
            await _commandBus.DispatchAsync(command, token);

            var url = Url.RouteUrl("ShortDocumentLink", new
            {
                base62 = new Base62(command.Id).ToString()
            });
            return new CreateDocumentResponse(url, score >= Privileges.Post);
        }


        [HttpGet(Name = "Documents")]
        public async Task<WebResponseWithFacet<DocumentFeedDto>> AggregateAllCoursesAsync(
           [FromQuery]DocumentRequestAggregate request,
           [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
           CancellationToken token)
        {
            var page = request.Page;

            _userManager.TryGetLongUserId(User, out var userId);

            var query = new DocumentAggregateQuery(userId, page, request.Filter, profile.Country);
            var result = await _queryBus.QueryAsync(query, token);
            return GenerateResult(result, new
            {
                page = ++page,
                filter = request.Filter
            });
        }
        
        private WebResponseWithFacet<DocumentFeedDto> GenerateResult(
            DocumentFeedWithFacetDto result, object nextPageParams)
        {
            var p = result.Result.Select(s =>
            {
                var uri = _blobProvider.GetPreviewImageLink(s.Id, 0);
                var effect = ImageProperties.BlurEffect.None;
                var properties = new ImageProperties(uri, effect);
                var url = Url.ImageUrl(properties);
                s.Preview = url;
                return s;
            }).ToList();


            string nextPageLink = null;
            if (p.Count > 0)
            {
                nextPageLink = Url.RouteUrl("Documents", nextPageParams);
            }

            var filters = new List<IFilters>();
            if (result.Facet.Any())
            {
                var filter = new Filters<string>(nameof(DocumentRequestAggregate.Filter), _localizer["TypeFilterTitle"],
                    result.Facet.Select(s => new KeyValuePair<string, string>(s, s)));
                filters.Add(filter);
            }


            return new WebResponseWithFacet<DocumentFeedDto>
            {
                Result = p.Select(s =>
                {
                    if (s.Url == null)
                    {
                        s.Url = Url.DocumentUrl(s.University, s.Course, s.Id, s.Title);
                    }

                    s.Title = Path.GetFileNameWithoutExtension(s.Title);
                    return s;
                }),
                Filters = filters,
                NextPageLink = nextPageLink
            };
        }

        [HttpGet]
        public async Task<WebResponseWithFacet<DocumentFeedDto>> SpecificCourseAsync(
            [RequiredFromQuery]DocumentRequestCourse request,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new DocumentCourseQuery(userId, request.Page, request.Course, request.Filter,profile.Country);
            var result = await _queryBus.QueryAsync(query, token);
            return GenerateResult(result, new { page = ++request.Page, request.Course, request.Filter });
        }

        [HttpGet]
        public async Task<WebResponseWithFacet<DocumentFeedDto>> SearchInCourseAsync(
            [RequiredFromQuery]  DocumentRequestSearchCourse request,
            [ProfileModelBinder(ProfileServiceQuery.UniversityId | ProfileServiceQuery.Country)] UserProfile profile,
            [FromServices] IDocumentSearch searchProvider,
            CancellationToken token)
        {
            var query = new DocumentQuery(profile, request.Term, request.Course, false, request.Filter?.Where(w => !string.IsNullOrEmpty(w)))
            {
                Page = request.Page,
            };
            var resultTask = searchProvider.SearchDocumentsAsync(query, token);
            var votesTask = System.Threading.Tasks.Task.FromResult<Dictionary<long, VoteType>>(null);

            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetLongUserId(User);
                var queryTags = new UserVotesByCategoryQuery(userId);
                votesTask = _queryBus.QueryAsync<IEnumerable<UserVoteDocumentDto>>(queryTags, token)
                    .ContinueWith(
                    t2 =>
                    {
                        return t2.Result.ToDictionary(x => x.Id, s => s.Vote);
                    }, token);

            }

            await System.Threading.Tasks.Task.WhenAll(resultTask, votesTask);
            
            return GenerateResult(resultTask.Result, new
            {
                page = ++request.Page,
                request.Course,
                request.Term,
                request.University,
                request.Filter
            });
        }

        [HttpGet]
        public async Task<WebResponseWithFacet<DocumentFeedDto>> SearchInSpitballAsync(
          [RequiredFromQuery]  DocumentRequestSearch request,

            [ProfileModelBinder(ProfileServiceQuery.UniversityId | ProfileServiceQuery.Country | ProfileServiceQuery.Course)] UserProfile profile,
            [FromServices] IDocumentSearch searchProvider,
            CancellationToken token)
        {

            var query = new DocumentQuery(profile, request.Term, null,
                request.University != null, request.Filter?.Where(w => !string.IsNullOrEmpty(w)))
            {
                Page = request.Page,
            };
            var resultTask = searchProvider.SearchDocumentsAsync(query, token);
            var votesTask = System.Threading.Tasks.Task.FromResult<Dictionary<long, VoteType>>(null);

            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetLongUserId(User);
                var queryTags = new UserVotesByCategoryQuery(userId);
                votesTask = _queryBus.QueryAsync<IEnumerable<UserVoteDocumentDto>>(queryTags, token)
                    .ContinueWith(
                        t2 =>
                        {
                            return t2.Result.ToDictionary(x => x.Id, s => s.Vote);
                        }, token);

            }

            await System.Threading.Tasks.Task.WhenAll(resultTask, votesTask);
            return GenerateResult(resultTask.Result, new
            {
                page = ++request.Page,
                request.Term,
                request.University,
                request.Filter
            });
        }

        [HttpPost("vote"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> VoteAsync([FromBody]
            AddVoteDocumentRequest model,
            [FromServices] IStringLocalizer<SharedResource> resource,
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
            catch (NoEnoughScoreException)
            {
                string voteMessage = resource[$"{model.VoteType:G}VoteError"];
                ModelState.AddModelError(nameof(AddVoteDocumentRequest.Id), voteMessage);
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

            return Ok();
        }

        [HttpPost("price"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ChangePriceAsync([FromBody] ChangePriceRequest model, CancellationToken token)
        {

            if (model.Price < 0)
            {
                ModelState.AddModelError(string.Empty, _localizer["PriceNeedToBeGreaterOrEqualZero"]);
                return BadRequest(ModelState);
            }
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
                ModelState.AddModelError("error",_localizer["SomeOnePurchased"]);
                return BadRequest(ModelState);
            }
        }


        [HttpPost("dropBox"), Authorize]
        public async Task<UploadStartResponse> UploadDropBox([FromBody] DropBoxRequest model,
           [FromServices] IRestClient client,
           [FromServices] IDocumentDirectoryBlobProvider documentDirectoryBlobProvider,
           CancellationToken token)
        {
            var (stream, _) = await client.DownloadStreamAsync(model.Link, token);
            var blobName = BlobFileName(Guid.NewGuid(), model.Name);
            await documentDirectoryBlobProvider.UploadStreamAsync(blobName, stream, token: token);

            return new UploadStartResponse(blobName);
        }


        [NonAction]
        public override Task FinishUploadAsync(UploadRequestFinish model, string blobName, CancellationToken token)
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }

    }
}