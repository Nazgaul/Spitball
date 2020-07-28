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
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Query.Tutor;

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
        private readonly IUrlBuilder _urlBuilder;

        public CourseController(IQueryBus queryBus, UserManager<User> userManager, IUrlBuilder urlBuilder)
        {
            _queryBus = queryBus;
            _userManager = userManager;
            _urlBuilder = urlBuilder;
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<CourseDetailDto?>> GetCourseByIdAsync([FromRoute] long id, CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new CourseByIdQuery(id, userId);
            var result = await _queryBus.QueryAsync(query, token);
            if (result == null)
            {
                return NotFound();
            }
            result.TutorImage = _urlBuilder.BuildUserImageEndpoint(result.TutorId, result.TutorImage);
            result.Image = _urlBuilder.BuildCourseThumbnailEndPoint(result.Id);
            return result;
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
