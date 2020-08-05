using Cloudents.Command;
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
using Cloudents.Core.DTOs;
using Cloudents.Query.Users;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Interfaces;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
    public class ProfileController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly UserManager<User> _userManager;
        private readonly ICommandBus _commandBus;
        private readonly IUrlBuilder _urlBuilder;

        public ProfileController(IQueryBus queryBus, UserManager<User> userManager,
              ICommandBus commandBus, IUrlBuilder urlBuilder)
        {
            _queryBus = queryBus;
            _userManager = userManager;
            _commandBus = commandBus;
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





    }
}