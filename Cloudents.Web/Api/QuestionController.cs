using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Item.Commands.FlagItem;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Hubs;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AppClaimsPrincipalFactory = Cloudents.Web.Identity.AppClaimsPrincipalFactory;

namespace Cloudents.Web.Api
{

    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class QuestionController : ControllerBase
    {
        private readonly ICommandBus _commandBus;

        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<QuestionController> _localizer;

        public QuestionController(ICommandBus commandBus, UserManager<User> userManager,
            IStringLocalizer<QuestionController> localizer
           )
        {
            _commandBus = commandBus;
            _userManager = userManager;
            _localizer = localizer;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CreateQuestionAsync([FromBody]CreateQuestionRequest model,
            [ClaimModelBinder(AppClaimsPrincipalFactory.Score)] int score,
            [FromServices] IHubContext<SbHub> hubContext,
            CancellationToken token)
        {

            var userId = _userManager.GetLongUserId(User);
            var toasterMessage = _localizer["PostedQuestionToasterOk"];
            try
            {
                var command = new CreateQuestionCommand(model.Text,
                    userId, model.Course);
                await _commandBus.DispatchAsync(command, token);
            }
            catch (DuplicateRowException)
            {
                toasterMessage = _localizer["PostedQuestionToasterPending"];
            }
            catch (InsufficientFundException)
            {
                ModelState.AddModelError(string.Empty, _localizer["InSufficientFunds"]);
                return BadRequest(ModelState);
            }
            catch (QuotaExceededException)
            {
                ModelState.AddModelError(string.Empty, _localizer["QuestionFlood"]);
                return BadRequest(ModelState);
            }
            if (score < Privileges.Post)
            {
                toasterMessage = _localizer["PostedQuestionToasterPending"];
            }
            await hubContext.Clients.User(userId.ToString()).SendCoreAsync("Message", new object[]
            {
                new SignalRTransportType(SignalRType.System, SignalREventAction.Toaster, new
                    {
                        text = toasterMessage.Value
                    }
                )}, token);
            return Ok();
        }

       

        

        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<QuestionDetailDto>> GetQuestionAsync(long id,
            [FromServices] IQueryBus bus, CancellationToken token)
        {
            var retValTask = bus.QueryAsync(new QuestionDataByIdQuery(id), token);
            //var votesTask = Task.FromResult<Dictionary<Guid, VoteType>>(null);

            //if (User.Identity.IsAuthenticated)
            //{
            //    var userId = _userManager.GetLongUserId(User);
            //    var queryTags = new UserVotesQuestionQuery(userId, id);
            //    votesTask = bus.QueryAsync(queryTags, token)
            //        .ContinueWith(
            //            t2 =>
            //            {
            //                return t2.Result.ToDictionary(x => x.Id, s => s.Vote);
            //            }, token);

            //}

            await Task.WhenAll(retValTask);
            var retVal = retValTask.Result;
            if (retVal == null)
            {
                return NotFound();
            }

            //if (votesTask.Result == null)
            //{
            //    return retVal;
            //}

            //if (votesTask.Result.TryGetValue(default, out var p))
            //{
            //    retVal.Vote.Vote = p;
            //}

            //foreach (var answer in retVal.Answers)
            //{
            //    if (votesTask.Result.TryGetValue(answer.Id, out var p2))
            //    {
            //        answer.Vote.Vote = p2;
            //    }
            //}

            return retVal;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteQuestionAsync([FromRoute]DeleteQuestionRequest model, CancellationToken token)
        {
            try
            {
                var command = new DeleteQuestionCommand(model.Id, _userManager.GetLongUserId(User));
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        //[AllowAnonymous, HttpGet(Name = "QuestionSearch")]
        //public async Task<ActionResult<WebResponseWithFacet<QuestionFeedDto>>> GetQuestionsAsync(
        //    [FromQuery]QuestionsRequest model,
        //    [ProfileModelBinder(ProfileServiceQuery.Country | ProfileServiceQuery.UniversityId)] UserProfile profile,
        //    [FromServices] IQueryBus queryBus,
        //   CancellationToken token)
        //{
        //    var query = new QuestionsQuery(model.Term, model.Course, model.NeedUniversity, 
        //        model.Filter?.Where(w => w.HasValue).Select(s => s.Value), profile.Country, profile.UniversityId)
        //    {
        //        Page = model.Page
        //    };



        //    var votesTask = Task.FromResult<Dictionary<long, VoteType>>(null);

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var userId = _userManager.GetLongUserId(User);
        //        var queryTags = new UserVotesByCategoryQuery(userId);
        //        votesTask = queryBus.QueryAsync<IEnumerable<UserVoteQuestionDto>>(queryTags, token)
        //            .ContinueWith(
        //                t2 =>
        //                {
        //                    return t2.Result.ToDictionary(x => x.Id, s => s.Vote);
        //                }, token);

        //    }

        //    var taskResult = _questionSearch.SearchAsync(query, token);
        //    await Task.WhenAll(votesTask, taskResult);

        //    var result = taskResult.Result;
        //    string nextPageLink = null;
        //    if (result.Result.Count > 0)
        //    {
        //        nextPageLink = Url.NextPageLink("QuestionSearch", null, model);
        //    }

        //    return new WebResponseWithFacet<QuestionFeedDto>
        //    {
        //        Result = result.Result.Select(s =>
        //        {
        //            if (s != null && (votesTask?.Result != null && votesTask.Result.TryGetValue(s.Id, out var param)))
        //            {
        //                s.Vote.Vote = param;
        //            }

        //            return s;
        //        }),
        //        Filters = new IFilters[]
        //        {
        //            new Filters<string>(nameof(QuestionsRequest.Filter),_localizer["FilterTypeTitle"],
        //                result.FacetState.Select(s=> new KeyValuePair<string, string>(s.ToString("G"),s.GetEnumLocalization()))),
                  
        //        },
        //        NextPageLink = nextPageLink
        //    };
        //}


        //[HttpPost("vote")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesDefaultResponseType]
        //public async Task<IActionResult> VoteAsync(
        //    [FromBody] AddVoteQuestionRequest model,
        //    [FromServices] IStringLocalizer<SharedResource> resource,
        //    CancellationToken token)
        //{
        //    var userId = _userManager.GetLongUserId(User);
        //    try
        //    {
        //        var command = new AddVoteQuestionCommand(userId, model.Id, model.VoteType);

        //        await _commandBus.DispatchAsync(command, token);
        //        return Ok();
        //    }
        //    catch (NoEnoughScoreException)
        //    {
        //        string voteMessage = resource[$"{model.VoteType:G}VoteError"];
        //        ModelState.AddModelError(nameof(AddVoteDocumentRequest.Id), voteMessage);
        //        return BadRequest(ModelState);
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        ModelState.AddModelError(nameof(AddVoteDocumentRequest.Id), _localizer["VoteCantVote"]);
        //        return BadRequest(ModelState);
        //    }

        //    catch (NotFoundException)
        //    {
        //        return NotFound();
        //    }
        //}

        [HttpPost("flag")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> FlagAsync([FromBody] FlagQuestionRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            try
            {
                var command = new FlagQuestionCommand(userId, model.Id, model.FlagReason);
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (NoEnoughScoreException)
            {
                ModelState.AddModelError(nameof(AddVoteDocumentRequest.Id), _localizer["VoteNotEnoughScore"]);
                return BadRequest(ModelState);
            }
        }

        //[HttpPost("ask"), Consumes("multipart/form-data")]
        //public async Task<UploadAskFileResponse> UploadFileAsync([Required]IFormFile file,
        //CancellationToken token)
        //{
        //    string[] supportedImages = { ".jpg", ".png", ".gif", ".jpeg", ".bmp" };

        //    var userId = _userManager.GetUserId(User);

        //    var fileNames = new List<string>();
        //    //foreach (var formFile in files)
        //    //{
        //    if (!file.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
        //    {
        //        throw new ArgumentException("not an image");
        //    }

        //    var extension = Path.GetExtension(file.FileName);

        //    if (!supportedImages.Contains(extension, StringComparer.OrdinalIgnoreCase))
        //    {
        //        throw new ArgumentException("not an image");
        //    }

        //    using (var sr = file.OpenReadStream())
        //    {
        //        //Image.FromStream(sr);
        //        var fileName = $"{userId}.{Guid.NewGuid()}.{file.FileName}";
        //        await _blobProvider
        //            .UploadStreamAsync(fileName, sr, file.ContentType, TimeSpan.FromSeconds(60 * 24), token);

        //        fileNames.Add(fileName);
        //    }
        //    //}
        //    return new UploadAskFileResponse(fileNames);
        //}
    }
}