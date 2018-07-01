using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Course api controller
    /// </summary>
    [Produces("application/json")]
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
        [HttpGet, ValidateModel]
        public async Task<IActionResult> GetAsync([FromQuery]  CourseRequest model, CancellationToken token)
        {
            var result = await _courseProvider.SearchAsync(model.Term, model.UniversityId.GetValueOrDefault(), token).ConfigureAwait(false);
            return Ok(new
            {
                courses = result
            });
        }

       
    }
}
