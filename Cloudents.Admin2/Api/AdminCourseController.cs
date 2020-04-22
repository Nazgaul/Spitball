using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Extension;
using Cloudents.Query;
using Cloudents.Query.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminCourseController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public AdminCourseController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpPost("migrate")]
        public async Task<IActionResult> MigrateCourse([FromBody] MigrateCourseRequest model,
            CancellationToken token)
        {
            var command = new MigrateCourseCommand(model.CourseToKeep, model.CourseToRemove);
            await _commandBus.DispatchAsync(command, token);

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseRequest model, CancellationToken token)
        {
            var command = new CreateCourseCommand(model.Name,model.Subject);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
        

        [HttpGet]
        [HttpGet("search")]
        [Authorize]
        public async Task<IEnumerable<PendingCoursesDto>> GetNewCourses([FromQuery]CoursesRequest model
                , CancellationToken token)
        {

            var query = new CoursesQuery(
                model.State.GetValueOrDefault(ItemState.Ok),
                User.GetSbCountryClaim(),
                model.Search);
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }

      


        [HttpPost("approve")]
        public async Task<IActionResult> ApproveCourse([FromBody] ApproveCourseRequest model,
                CancellationToken token)
        {
            var command = new ApproveCourseCommand(model.Course, model.Subject);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }


        [HttpPost("rename")]
        public async Task<IActionResult> RenameCourse([FromBody] RenameCourseRequest model,
                CancellationToken token)
        {
            var command = new RenameCourseCommand(model.OldName, model.NewName);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }


        [HttpGet("subject")]
        [Authorize]
        public async Task<IEnumerable<string>> GetSubjects(CancellationToken token)
        {
            var query = new SubjectsQuery(User.GetIdClaim());
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }


        [HttpDelete("{name}")]
        [Authorize]
        public async Task<IActionResult> DeleteCourse(string name,
                CancellationToken token)
        {
            var command = new DeleteCourseCommand(name);
            try
            {
                await _commandBus.DispatchAsync(command, token);
            }
            catch(SqlConstraintViolationException e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }
    }
}
