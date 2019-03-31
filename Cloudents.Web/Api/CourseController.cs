using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Query;
using Cloudents.Query.Query;

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
        private readonly UserManager<RegularUser> _userManager;
        private readonly SignInManager<RegularUser> _signInManager;

        public CourseController(IQueryBus queryBus, ICommandBus commandBus, UserManager<RegularUser> userManager,
            SignInManager<RegularUser> signInManager)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Perform course search
        /// </summary>
        /// <param name="model">params</param>
        /// <param name="token"></param>
        /// <returns>list of courses filter by input</returns>
        [Route("search")]
        [HttpGet]
        [ResponseCache(Duration = TimeConst.Hour,Location = ResponseCacheLocation.Any,VaryByQueryKeys = new []{nameof(CourseRequest.Term) })]
        public async Task<CoursesResponse> GetAsync([FromQuery]CourseRequest model,
            CancellationToken token)
        {
            var query = new CourseSearchQuery(model.Term);
            var result = await _queryBus.QueryAsync(query, token);
            return new CoursesResponse
            {
                Courses = result
            };
        }

        [HttpPost]
        public async Task<IActionResult> SetCoursesAsync([FromBody] SetCourseRequest[] model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new UserJoinCoursesCommand(model.Select(s=>s.Name), userId);
            await _commandBus.DispatchAsync(command, token);
            var user = await _userManager.GetUserAsync(User);
            await _signInManager.RefreshSignInAsync(user);
            return Ok();
        }

        //[HttpPost("tutor")]
        //public async Task<IActionResult> SetTutorCoursesAsync([FromBody] SetCourseRequest[] model, CancellationToken token)
        //{
        //    var userId = _userManager.GetLongUserId(User);
        //    var command = new AssignCoursesToTutorCommand(model.Select(s => s.Name), userId);
        //    await _commandBus.DispatchAsync(command, token);
        //    var user = await _userManager.GetUserAsync(User);
        //    await _signInManager.RefreshSignInAsync(user);
        //    return Ok();
        //}
    }
}
