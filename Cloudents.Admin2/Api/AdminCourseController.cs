using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminCourseController: ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public AdminCourseController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet("courses")]
        public async Task<IEnumerable<NewCourseDto>> Get(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            var retVal = await _queryBus.QueryAsync<IList<NewCourseDto>>(query, token);
            return retVal;
        }


        [HttpPost("migrate")]
        public async Task<IActionResult> MigrateCourse([FromBody] MigrateCourseRequest model,
            CancellationToken token)
        {
            var command = new MigrateCourseCommand(model.CourseToKeep, model.CourseToRemove);
            var deleteCommand = new DeleteCourseCommand(model.CourseToRemove);
            await _commandBus.DispatchAsync(command, token);
            await _commandBus.DispatchAsync(deleteCommand, token);
            return Ok();
        }

        /// <summary>
        /// Perform course search
        /// </summary>
        /// <param name="course">course</param>
        /// <param name="token"></param>
        /// <returns>list of courses filter by input</returns>
        [Route("search")]
        [HttpGet]
        public async Task<CoursesResponse> GetAsync([FromQuery(Name = "course")]string course,
            CancellationToken token)
        {
            var query = new CourseSearchQuery(course);
            var result = await _queryBus.QueryAsync(query, token);
            return new CoursesResponse
            {
                Courses = result
            };
        }

        [HttpGet("newCourses")]
        public async Task<IEnumerable<PendingCoursesDto>> GetNewCourses(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            var retVal = await _queryBus.QueryAsync<IList<PendingCoursesDto>>(query, token);
            return retVal;
        }

        [HttpGet("allCourses")]
        public async Task<IEnumerable<string>> GetAllCourses(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            var retVal = await _queryBus.QueryAsync<IList<string>>(query, token);
            return retVal;
        }


        [HttpPost("approve")]
        public async Task<IActionResult> ApproveCourse([FromBody] ApproveCourseRequest model,
                CancellationToken token)
        {
            var command = new ApproveCourseCommand(model.Course);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> ApproveCourse(string name,
                CancellationToken token)
        {
            var command = new DeleteCourseCommand(name);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}
