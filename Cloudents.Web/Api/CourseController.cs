using Cloudents.Core.Entities;
using Cloudents.Query;
using Cloudents.Query.Courses;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Course api controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController, Authorize]
    public class CourseController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly UserManager<User> _userManager;

        public CourseController(IQueryBus queryBus, UserManager<User> userManager
            )
        {
            _queryBus = queryBus;
            _userManager = userManager;
        }

        /// <summary>
        /// Perform course search - we can't put cache because the user can re-enter the page
        /// </summary>
        /// <param name="request">params</param>
        /// <param name="token"></param>
        /// <returns>list of courses filter by input</returns>
        [HttpGet("search")]
        [AllowAnonymous]

        public async Task<CoursesResponse> GetAsync(
           [FromQuery] CourseSearchRequest request,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);


            var query = new CourseSearchQuery(userId, request.Term);
            var temp = await _queryBus.QueryAsync(query, token);


            return new CoursesResponse(temp);
        }
    }
}
