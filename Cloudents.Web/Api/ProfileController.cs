﻿using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Microsoft.AspNetCore.Http;
using Cloudents.Web.Models;
using Cloudents.Query.Users;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Query.Tutor;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
    public class ProfileController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly UserManager<User> _userManager;
        private readonly IUrlBuilder _urlBuilder;
        private readonly ICommandBus _commandBus;

        public ProfileController(IQueryBus queryBus, UserManager<User> userManager,
             IUrlBuilder urlBuilder, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _userManager = userManager;
            _urlBuilder = urlBuilder;
            _commandBus = commandBus;
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
            var query = new UserProfileAboutQuery(id);
            var res = await _queryBus.QueryAsync(query, token);
            return res;
        }



        [HttpGet("{id:long}/documents")]
        [ProducesResponseType(200)]

        public async Task<WebResponseWithFacet<DocumentFeedDto>> GetDocumentsAsync(
            [FromQuery] ProfileDocumentsRequest request, CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new UserDocumentsQuery(request.Id, request.Page, request.PageSize,
                request.DocumentType, request.Course,userId);
            var retVal = await _queryBus.QueryAsync(query, token);
          
            return new WebResponseWithFacet<DocumentFeedDto>()
            {
                Result = retVal.Result.Select(s =>
                {
                    s.Url = Url.DocumentUrl(s.Course, s.Id, s.Title);
                    s.Preview = _urlBuilder.BuildDocumentThumbnailEndpoint(s.Id);
                    return s;
                }),
                Count = retVal.Count
            };
        }

        [HttpGet("{id:long}/studyRoom")]
        public async Task<IEnumerable<FutureBroadcastStudyRoomDto>> GetUpcomingEventsAsync(long id, CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new TutorUpcomingBroadcastStudyRoomQuery(id, userId);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpPost("{id:long}/studyRoom"), Authorize]
        public async Task EnrollUpcomingEventAsync(EnrollStudyRoomRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new EnrollStudyRoomBroadCastCommand(userId,model.StudyRoomId);
            await _commandBus.DispatchAsync(command, token);
            
        }
        

        [HttpPost("follow"), Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> FollowAsync([FromBody] FollowRequest model,  CancellationToken token)
        {
            var user = _userManager.GetLongUserId(User);
            if (model.Id == user)
            {
                return BadRequest();
            }
            var command = new FollowUserCommand(model.Id, user);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpDelete("unFollow/{id}"), Authorize]
        public async Task<IActionResult> UnFollowAsync([FromRoute] UnFollowRequest model, [FromServices] ICommandBus commandBus, CancellationToken token)
        {
            var user = _userManager.GetLongUserId(User);
            var command = new UnFollowUserCommand(model.Id, user);
            await commandBus.DispatchAsync(command, token);
            return Ok();
        }



    }
}