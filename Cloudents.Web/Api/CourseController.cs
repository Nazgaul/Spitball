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
    [Produces("application/json")]
    [Route("api/Course")]
    public class CourseController : Controller
    {
        private readonly ICourseSearch _courseProvider;
        private readonly ICommandBus _command;

        public CourseController(ICourseSearch courseProvider, ICommandBus command)
        {
            _courseProvider = courseProvider;
            _command = command;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string term, long universityId, CancellationToken token)
        {
            if (universityId == 0)
            {
                throw new ArgumentException(nameof(universityId));
            }
            var result = await _courseProvider.SearchAsync(term, universityId, token).ConfigureAwait(false);
            return Json(result);
        }

        [ValidateModel]
        [HttpPost]
        public async Task<IActionResult> Post(CreateCourse model,CancellationToken token)
        {
            if (!model.University.HasValue)
            {
                throw new ArgumentNullException(nameof(model.University));
            }

            var command = new CreateCourseCommand(model.Name, model.University.Value);
            var response = await _command.DispatchAsync<CreateCourseCommand, CreateCourseCommandResult>(command, token).ConfigureAwait(false);
            return Json( new
            {
                response.Id
            });
        }
    }
}