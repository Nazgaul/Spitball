using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Identity;

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
        private readonly ICourseSearch _courseProvider;
        private readonly ICommandBus _commandBus;
        private readonly UserManager<User> _userManager;


        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="courseProvider"></param>
        /// <param name="commandBus"></param>
        /// <param name="userManager"></param>
        public CourseController(ICourseSearch courseProvider, ICommandBus commandBus, UserManager<User> userManager)
        {
            _courseProvider = courseProvider;
            _commandBus = commandBus;
            _userManager = userManager;
        }

        /// <summary>
        /// Perform course search
        /// </summary>
        /// <param name="model">params</param>
        /// <param name="universityId">This params comes from claim and not from api - value is ignored</param>
        /// <param name="token"></param>
        /// <returns>list of courses filter by input</returns>
        [Route("search")]
        [HttpGet]
        public async Task<CoursesResponse> GetAsync([FromQuery]CourseRequest model,
            [Required(ErrorMessage = "NeedUniversity"), ClaimModelBinder(AppClaimsPrincipalFactory.University)] long universityId,
            CancellationToken token)
        {
            var result = await _courseProvider.SearchAsync(model.Term, universityId, token).ConfigureAwait(false);
            return new CoursesResponse
            {
                Courses = result
            };
        }

        /// <summary>
        /// Create academic course
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns>The id of the course created</returns>
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateAcademicBoxAsync([FromBody]CreateCourseRequest model,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new CreateCourseCommand(model.CourseName, userId);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
            //return new CoursesCreateResponse
            //{
            //    Id = command.Id
            //};
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignUniversityAsync([FromBody] AssignCourseRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new AssignCourseToUserCommand(userId, model.CourseId);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }
    }
}
