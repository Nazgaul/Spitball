using Cloudents.Core.Entities;
using Cloudents.Query;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs;
using Cloudents.Query.Users;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

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
        public async Task<UserProfileReviewsDto> GetUserReviewsAsync(long id, CancellationToken token)
        {
            var query = new UserProfileReviewsQuery(id);
            var res = await _queryBus.QueryAsync(query, token);
            return res;
        }

        [HttpGet("{id:long}/courses")]
        public async Task<IEnumerable<CourseDto>> GetCourses([FromRoute] long id, CancellationToken token)
        {
            var query = new UserCoursesQuery(id);
            var res = await _queryBus.QueryAsync(query, token);
            return res.Select(s =>
            {
                s.Image = _urlBuilder.BuildCourseThumbnailEndPoint(s.Id, s.Version);
                return s;
            });
        }


        [HttpPost("follow"), Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> FollowAsync([FromBody] FollowRequest model, CancellationToken token)
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
        public async Task<IActionResult> UnFollowAsync([FromRoute] UnFollowRequest model, CancellationToken token)
        {
            var user = _userManager.GetLongUserId(User);
            var command = new UnFollowUserCommand(model.Id, user);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

    }
}