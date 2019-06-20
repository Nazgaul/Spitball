﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly UserManager<User> _userManager;


        public ProfileController(IQueryBus queryBus, UserManager<User> userManager)
        {
            _queryBus = queryBus;
            _userManager = userManager;
        }

        // GET
        [HttpGet("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        
        public async Task<ActionResult<UserProfileDto>> GetAsync(long id, CancellationToken token)
        {
            var query = new UserProfileQuery(id);
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
            var retValTask = _queryBus.QueryAsync<IEnumerable<QuestionFeedDto>>(query, token);

            return await MergeFeedWithVotes(retValTask, token);
        }

        private async Task<IEnumerable<QuestionFeedDto>> MergeFeedWithVotes(Task<IEnumerable<QuestionFeedDto>> retValTask, CancellationToken token)
        {
            var votesTask = Task.FromResult<Dictionary<long, VoteType>>(null);
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetLongUserId(User);
                var queryTags = new UserVotesByCategoryQuery(userId);
                votesTask = _queryBus.QueryAsync<IEnumerable<UserVoteQuestionDto>>(queryTags, token)
                    .ContinueWith(
                        t2 => { return t2.Result.ToDictionary(x => x.Id, s => s.Vote); }, token);
            }

            await Task.WhenAll(retValTask, votesTask);
            if (votesTask.Result == null)
            {
                return retValTask.Result;
            }

            return retValTask.Result.Select(s =>
            {
                if (votesTask.Result.TryGetValue(s.Id, out var p))
                {
                    s.Vote.Vote = p;
                }

                return s;
            });
        }

        // GET
        [HttpGet("{id:long}/answers")]
        [ProducesResponseType(200)]

        public async Task<IEnumerable<QuestionFeedDto>> GetAnswersAsync(long id, int page, CancellationToken token)
        {
            var query = new UserAnswersByIdQuery(id, page);
            var retValTask = _queryBus.QueryAsync<IEnumerable<QuestionFeedDto>>(query, token);
            return await MergeFeedWithVotes(retValTask, token);
        }

        [HttpGet("{id:long}/documents")]
        [ProducesResponseType(200)]

        public async Task<IEnumerable<DocumentFeedDto>> GetDocumentsAsync(long id, int page, CancellationToken token)
        {
            var query = new UserDataPagingByIdQuery(id, page);
            var retValTask = _queryBus.QueryAsync<IEnumerable<DocumentFeedDto>>(query, token);

            var votesTask = Task.FromResult<Dictionary<long, VoteType>>(null);
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetLongUserId(User);
                var queryTags = new UserVotesByCategoryQuery(userId);
                votesTask = _queryBus.QueryAsync<IEnumerable<UserVoteDocumentDto>>(queryTags, token)
                    .ContinueWith(
                        t2 => { return t2.Result.ToDictionary(x => x.Id, s => s.Vote); }, token);
            }

            await Task.WhenAll(retValTask, votesTask);
            return retValTask.Result.Select(s =>
            {
                s.Url = Url.DocumentUrl(s.University, s.Course, s.Id, s.Title);

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
                votesTask = _queryBus.QueryAsync<IEnumerable<UserVoteDocumentDto>>(queryTags, token)
                    .ContinueWith(
                        t2 => { return t2.Result.ToDictionary(x => x.Id, s => s.Vote); }, token);
            }

            await Task.WhenAll(retValTask, votesTask);
            return retValTask.Result.Select(s =>
            {
                s.Url = Url.DocumentUrl(s.University, s.Course, s.Id, s.Title);

                if (votesTask.Result != null && votesTask.Result.TryGetValue(s.Id, out var p))
                {
                    s.Vote.Vote = p;
                }
                return s;
            });
        }

    }
}