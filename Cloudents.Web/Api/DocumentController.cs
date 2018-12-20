using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Item.Commands.FlagItem;
using Cloudents.Command.Votes.Commands.AddVoteDocument;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Hubs;
using Cloudents.Web.Identity;
using Cloudents.Web.Resources;
using Microsoft.AspNetCore.SignalR;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly UserManager<RegularUser> _userManager;
        private readonly IBlobProvider<DocumentContainer> _blobProvider;
        private readonly IStringLocalizer<DocumentController> _localizer;
        private readonly IProfileUpdater _profileUpdater;



        public DocumentController(IQueryBus queryBus,
             ICommandBus commandBus, UserManager<RegularUser> userManager,
            IBlobProvider<DocumentContainer> blobProvider,
            IStringLocalizer<DocumentController> localizer,
            IProfileUpdater profileUpdater)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _userManager = userManager;
            _blobProvider = blobProvider;
            _localizer = localizer;
            _profileUpdater = profileUpdater;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentPreviewResponse>> GetAsync(long id,
            [FromServices] IQueueProvider queueProvider,
            [FromServices] IBlobProvider blobProvider,

            CancellationToken token)
        {
            var query = new DocumentById(id);
            var tModel = _queryBus.QueryAsync<DocumentDetailDto>(query, token);
            var filesTask = _blobProvider.FilesInDirectoryAsync("preview-", query.Id.ToString(), token);

            var tQueue = queueProvider.InsertMessageAsync(new UpdateDocumentNumberOfViews(id), token);
            await Task.WhenAll(tModel, filesTask, tQueue);

            var model = tModel.Result;
            var files = filesTask.Result.Select(s => blobProvider.GeneratePreviewLink(s, 20));
            if (model == null)
            {
                return NotFound();
            }

            if (!filesTask.Result.Any())
            {
                await queueProvider.InsertBlobReprocessAsync(id);
                //var queue = queueClient.GetQueueReference("generate-blob-preview");
                //await queue.AddMessageAsync(new CloudQueueMessage(item.Id.ToString()));
            }
            return new DocumentPreviewResponse(model, files);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<CreateDocumentResponse>> CreateDocumentAsync([FromBody]CreateDocumentRequest model,
            [ProfileModelBinder(ProfileServiceQuery.University)] UserProfile profile,
            [FromServices] IHubContext<SbHub> hubContext,
            [ClaimModelBinder(AppClaimsPrincipalFactory.Score)] int score,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            if (!model.BlobName.StartsWith("file-", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.Name), "Invalid file name");
                return BadRequest(ModelState);
            }
            var command = new CreateDocumentCommand(model.BlobName, model.Name, model.Type,
                model.Course, model.Tags, userId, model.Professor);
            await _commandBus.DispatchAsync(command, token);

            var url = Url.DocumentUrl(profile.University.Name, model.Course, command.Id, model.Name);

            var localizerKey = score < Privileges.Post ? "CreatePending" : "CreateOk";
            await hubContext.Clients.User(userId.ToString()).SendCoreAsync("Message", new object[]
            {
                new SignalRTransportType(SignalRType.System, SignalREventAction.Toaster, new
                    {
                        text = _localizer[localizerKey].Value
                    }
                )
            }, token);
            return new CreateDocumentResponse(url);
        }


        /// <summary>
        /// Search document vertical result
        /// </summary>
        /// <param name="model"></param>
        /// <param name="profile">User profile - server generated</param>
        /// <param name="ilSearchProvider"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet(Name = "DocumentSearch"), AllowAnonymous]
        [ResponseCache(Duration = TimeConst.Minute, VaryByQueryKeys = new[] { "*" }, Location = ResponseCacheLocation.Client)]
        public async Task<WebResponseWithFacet<DocumentFeedDto>> SearchDocumentAsync([FromQuery] DocumentRequest model,
            [ProfileModelBinder(ProfileServiceQuery.University | ProfileServiceQuery.Country |
                                ProfileServiceQuery.Course | ProfileServiceQuery.Tag)]
            UserProfile profile,
            [FromServices] IDocumentSearch ilSearchProvider,
            CancellationToken token)
        {
            model = model ?? new DocumentRequest();
            var query = new DocumentQuery(model.Course, profile, model.Term,
                model.Page.GetValueOrDefault(), model.Filter?.Where(w => w.HasValue).Select(s => s.Value));


            var queueTask = _profileUpdater.AddTagToUser(model.Term, User, token);
            var resultTask = ilSearchProvider.SearchDocumentsAsync(query, token);

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

            await Task.WhenAll(resultTask, queueTask, votesTask);
            var result = resultTask.Result;
            var p = result;
            string nextPageLink = null;
            if (p.Count > 0)
            {
                nextPageLink = Url.NextPageLink("DocumentSearch", null, model);
            }

            var filters = new List<IFilters>
            {
                new Filters<string>(nameof(DocumentRequest.Filter), _localizer["TypeFilterTitle"],
                    EnumExtension.GetValues<DocumentType>()
                        .Where(w => w.GetAttributeValue<PublicValueAttribute>() != null)
                        .Select(s => new KeyValuePair<string, string>(s.ToString("G"), s.GetEnumLocalization())))
            };


            if (profile.Courses != null)
            {
                filters.Add(new Filters<string>(nameof(DocumentRequest.Course),
                    _localizer["CoursesFilterTitle"],
                    profile.Courses.Select(s => new KeyValuePair<string, string>(s, s))));
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
    }


}