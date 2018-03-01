using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.MobileApi.Extensions;
using Cloudents.MobileApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.MobileApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Course api controller
    /// </summary>
    [Route("api/[controller]")]
    public class CourseController : Controller
    {
        private readonly ICourseSearch _courseProvider;
        private readonly ICommandBus _command;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="courseProvider"></param>
        /// <param name="command"></param>
        public CourseController(ICourseSearch courseProvider, ICommandBus command)
        {
            _courseProvider = courseProvider;
            _command = command;
        }

        /// <summary>
        /// Perform course search
        /// </summary>
        /// <param name="model">params</param>
        /// <param name="token"></param>
        /// <returns>list of courses filter by input</returns>
        /// <exception cref="ArgumentException">university is empty</exception>
        [Route("search")]
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery]  CourseRequest model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _courseProvider.SearchAsync(model.Term, model.UniversityId.GetValueOrDefault(), token).ConfigureAwait(false);
            return Ok(new
            {
                courses = result
            });
        }

        /// <summary>
        /// Create academic course
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns>The id of the course created</returns>
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateAcademicBoxAsync([FromBody]CreateCourseRequest model, CancellationToken token)
        {
            if (!ModelState.IsValid || !model.University.HasValue)
            {
                return BadRequest(ModelState);
            }
            var command = new CreateCourseCommand(model.CourseName, model.University.Value);
            var response = await _command.DispatchAsync<CreateCourseCommand, CreateCourseCommandResult>(command, token).ConfigureAwait(false);
            return Ok(new
            {
                response.Id
            });
        }
    }
}
