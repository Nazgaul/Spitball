using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Query;

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
        private readonly ICommandBus _commandBus;
        private readonly UserManager<User> _userManager;


        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="courseProvider"></param>
        /// <param name="commandBus"></param>
        /// <param name="userManager"></param>
        public CourseController(IQueryBus queryBus, ICommandBus commandBus, UserManager<User> userManager)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _userManager = userManager;
        }

        /// <summary>
        /// Perform course search
        /// </summary>
        /// <param name="model">params</param>
        /// <param name="token"></param>
        /// <returns>list of courses filter by input</returns>
        [Route("search")]
        [HttpGet]
        public async Task<CoursesResponse> GetAsync([FromQuery]CourseRequest model,

            CancellationToken token)
        {
            var query = new CourseSearchQuery(model.Term);
            var result = await _queryBus.QueryAsync(query, token);
            //var result = await _courseProvider.SearchAsync(model.Term, token).ConfigureAwait(false);
            return new CoursesResponse
            {
                Courses = result
            };
        }


        [HttpPost("assign")]
        public async Task<IActionResult> AssignUniversityAsync([FromBody] AssignCourseRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new AssignCourseToUserCommand(model.Name, userId);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }
    }
}
