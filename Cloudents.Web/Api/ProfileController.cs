using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly UserManager<User> _userManager;
        private readonly IUrlBuilder _urlBuilder;


        public ProfileController(IQueryBus queryBus, UserManager<User> userManager,
             IUrlBuilder urlBuilder)
        {
            _queryBus = queryBus;
            _userManager = userManager;
            _urlBuilder = urlBuilder;
        }

        // GET
        [HttpGet("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]

        public async Task<ActionResult<UserProfileDto>> GetAsync(long id,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new UserProfileQuery(id, userId);
            var retVal = await _queryBus.QueryAsync(query, token);
            if (retVal == null)
            {
                return NotFound();
            }
            return retVal;
        }

        [HttpGet("{id:long}/about")]
        public async Task<UserProfileAboutDto> GetAboutAsync(long id, CancellationToken token)
        {
            //var user = _userManager.GetU
            //_userManager.IsInRoleAsync()
            var query = new UserProfileAboutQuery(id);
            return await _queryBus.QueryAsync(query, token);
        }

        // GET
        [HttpGet("{id:long}/questions")]
        [ProducesResponseType(200)]

        public async Task<IEnumerable<QuestionFeedDto>> GetQuestionsAsync(long id, int page, CancellationToken token)
        {
            var query = new UserDataPagingByIdQuery(id, page);
            return await _queryBus.QueryAsync<IEnumerable<QuestionFeedDto>>(query, token);

        }

        // GET
        [HttpGet("{id:long}/answers")]
        [ProducesResponseType(200)]

        public async Task<IEnumerable<QuestionFeedDto>> GetAnswersAsync(long id, int page, CancellationToken token)
        {
            var query = new UserAnswersByIdQuery(id, page);
            return await _queryBus.QueryAsync<IEnumerable<QuestionFeedDto>>(query, token);
        }

        [HttpGet("{id:long}/documents")]
        [ProducesResponseType(200)]

        public async Task<IEnumerable<DocumentFeedDto>> GetDocumentsAsync(
            long id, int page,

            CancellationToken token)
        {
            var query = new UserDataPagingByIdQuery(id, page);
            var retValTask = _queryBus.QueryAsync<IEnumerable<DocumentFeedDto>>(query, token);

            var votesTask = Task.FromResult<Dictionary<long, VoteType>>(null);
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetLongUserId(User);
                var queryTags = new UserVotesByCategoryQuery(userId);
                votesTask = _queryBus.QueryAsync(queryTags, token)
                    .ContinueWith(
                        t2 => { return t2.Result.ToDictionary(x => x.Id, s => s.Vote); }, token);
            }

            await Task.WhenAll(retValTask, votesTask);
            return retValTask.Result.Select(s =>
            {
                s.Url = Url.DocumentUrl(s.Course, s.Id, s.Title);
                s.Preview = _urlBuilder.BuildDocumentThumbnailEndpoint(s.Id);
                if (votesTask.Result != null && votesTask.Result.TryGetValue(s.Id, out var p))
                {
                    s.Vote.Vote = p;
                }

                return s;
            });
        }

        [HttpGet("{id:long}/purchaseDocuments")]
        [ProducesResponseType(200)]

        public async Task<IEnumerable<DocumentFeedDto>> GetPurchaseDocumentsAsync(long id, int page, CancellationToken token)
        {
            var query = new UserPurchaseDocumentByIdQuery(id, page);
            var retValTask = _queryBus.QueryAsync(query, token);

            var votesTask = Task.FromResult<Dictionary<long, VoteType>>(null);
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetLongUserId(User);
                var queryTags = new UserVotesByCategoryQuery(userId);
                votesTask = _queryBus.QueryAsync(queryTags, token)
                    .ContinueWith(
                        t2 => { return t2.Result.ToDictionary(x => x.Id, s => s.Vote); }, token);
            }

            await Task.WhenAll(retValTask, votesTask);
            return retValTask.Result.Select(s =>
            {
                s.Url = Url.DocumentUrl(s.Course, s.Id, s.Title);
                s.Preview = _urlBuilder.BuildDocumentThumbnailEndpoint(s.Id);
                if (votesTask.Result != null && votesTask.Result.TryGetValue(s.Id, out var p))
                {
                    s.Vote.Vote = p;
                }
                return s;
            });
        }

        [HttpPost("follow"), Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> FollowAsync(long id, [FromServices] ICommandBus commandBus, CancellationToken token)
        {
            var user = _userManager.GetLongUserId(User);
            if (id == user)
            {
                return BadRequest();
            }
            var command = new FollowUserCommand(id, user);
            await commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpDelete("unFollow"), Authorize]
        public async Task<IActionResult> UnFollowAsync(long id, [FromServices] ICommandBus commandBus, CancellationToken token)
        {
            var user = _userManager.GetLongUserId(User);
            var command = new UnFollowUserCommand(id, user);
            await commandBus.DispatchAsync(command, token);
            return Ok();
        }



    }
}