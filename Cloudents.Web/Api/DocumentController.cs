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
using Cloudents.Core.Query;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Cloudents.Web.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Web.Framework;
using Wangkanai.Detection;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController, Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly UserManager<RegularUser> _userManager;
        private readonly IDocumentDirectoryBlobProvider _blobProvider;
        private readonly IStringLocalizer<DocumentController> _localizer;
        //private readonly IProfileUpdater _profileUpdater;



        public DocumentController(IQueryBus queryBus,
             ICommandBus commandBus, UserManager<RegularUser> userManager,
             IDocumentDirectoryBlobProvider blobProvider,
            IStringLocalizer<DocumentController> localizer
            )
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _userManager = userManager;
            _blobProvider = blobProvider;
            _localizer = localizer;
        }

        [HttpGet("{id}"), AllowAnonymous]
        public async Task<ActionResult<DocumentPreviewResponse>> GetAsync(long id,
            [FromServices] IQueueProvider queueProvider,
            [FromServices] ICrawlerResolver crawlerResolver,
            [FromServices] IConfiguration configuration,
            [FromServices] IBlobProvider blobProvider,
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
            var textTask = Task.FromResult<string>(null);
            if (crawlerResolver.Crawler != null)
            {
                textTask = _blobProvider.DownloadTextAsync("text.txt", query.Id.ToString(), token);
            }
            var prefix = "preview-";
            if (!model.IsPurchased)
            {
                prefix = "blur-";
            }
            var filesTask = _blobProvider.FilesInDirectoryAsync(prefix, query.Id.ToString(), token);
            //var range = Enumerable.Range(0, model.PageCount);

            //var files = range.Select(page =>
            //{
            //    var properties = new ImageProperties(model.Id, page, !model.IsPurchased);

            //    var hash = properties.Encrypt();

            //    var uri = QueryHelpers.AddQueryString(
            //        $"{configuration["functionCdnEndpoint"]}/api/image/{Base64UrlTextEncoder.Encode(hash)}",
            //        new Dictionary<string, string>()
            //    {
            //        {"width","880" },
            //        {"height","1270"},
            //        {"mode","Max" }
            //    });
            //    return new Uri(uri);
            //});

            await Task.WhenAll(tQueue, textTask, filesTask);
            var files = (filesTask.Result.Select(s => blobProvider.GeneratePreviewLink(s, TimeSpan.FromMinutes(20)))).ToList();
            if (!files.Any())
            {
                await queueProvider.InsertBlobReprocessAsync(id);
            }
            return new DocumentPreviewResponse(model, files, textTask.Result);
        }

        [HttpPost]
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


        [HttpGet]
        public async Task<string> All()
        {
            return "All";
        }

        [HttpGet, IgnoreFromQueryActionConstraint("term")]
        public async Task<string> Course(
            [RequiredFromQuery] string course)
        {
            return "Course" + course;
        }

        [HttpGet]
        public async Task<string> Search(
            [RequiredFromQuery] string term, string course)
        {
            return "Search" + term;
        }

        /// <summary>
        /// Search document vertical result
        /// </summary>
        /// <param name="model"></param>
        /// <param name="searchProvider"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("{se}",Name = "DocumentSearch"), AllowAnonymous]
        //TODO:We have issue in here because of changing course we need to invalidate the query.
        //[ResponseCache(Duration = TimeConst.Second * 15, VaryByQueryKeys = new[] { "*" }, Location = ResponseCacheLocation.Client)]
        public async Task<WebResponseWithFacet<DocumentFeedDto>> SearchDocumentAsync(
            [FromQuery] DocumentRequest model,
            [FromServices] IDocumentSearch searchProvider,
            CancellationToken token)
        {

            model = model ?? new DocumentRequest();
            var query = new DocumentQuery(model.Profile, model.Term, model.Course, !string.IsNullOrEmpty(model.University), model.Filter?.Where(w => !string.IsNullOrEmpty(w)))
            {
                Page = model.Page.GetValueOrDefault(),
            };

            var resultTask = searchProvider.SearchDocumentsAsync(query, token);
            var votesTask = Task.FromResult<Dictionary<long, VoteType>>(null);

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

            await Task.WhenAll(resultTask, votesTask);
            var result = resultTask.Result;
            var p = result.Result.ToList();
            string nextPageLink = null;
            if (p.Count > 0)
            {
                nextPageLink = Url.NextPageLink("DocumentSearch", null, model);
            }

            var filters = new List<IFilters>();
            if (result.Facet.Any())
            {
                var filter = new Filters<string>(nameof(DocumentRequest.Filter), _localizer["TypeFilterTitle"],
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

                    if (votesTask?.Result != null && votesTask.Result.TryGetValue(s.Id, out var param))
                    {
                        s.Vote.Vote = param;
                    }
                    s.Title = Path.GetFileNameWithoutExtension(s.Title);
                    return s;
                }),
                Filters = filters,
                NextPageLink = nextPageLink
            };
        }

        [HttpPost("vote")]
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

        [HttpPost("flag")]
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


        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseAsync([FromBody] PurchaseDocumentRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            try
            {
                var command = new PurchaseDocumentCommand(model.Id, userId);
                await _commandBus.DispatchAsync(command, token);
            }
            catch (InsufficientFundException)
            {
                ModelState.AddModelError(string.Empty, _localizer["InSufficientFunds"]);
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpPost("price")]
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

        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteDocumentAsync([FromRoute]DeleteDocumentRequest model, CancellationToken token)
        {
            try
            {
                var command = new DeleteDocumentCommand(model.Id, _userManager.GetLongUserId(User));
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

    }
}