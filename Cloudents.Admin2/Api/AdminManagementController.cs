using Cloudents.Command;
using Cloudents.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using System.Threading;
using Cloudents.Query.Query.Admin;
using Cloudents.Admin2.Models;
using Cloudents.Command.Command.Admin;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminManagementController: ControllerBase
    {
       private readonly IQueryBus _queryBus;
       private readonly ICommandBus _commandBus;

        public AdminManagementController(IQueryBus queryBus, ICommandBus commandBus)
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

        [HttpPost("courses")]
        public async Task<IActionResult> MigrateCourse([FromBody] MigrateCourseRequest model, 
            CancellationToken token)
        {
            var command = new MigrateCourseCommand(model.CourseToKeep, model.CourseToRemove);
            var deleteCommand = new DeleteCourseCommand(model.CourseToRemove);
            await _commandBus.DispatchAsync(command, token);
            await _commandBus.DispatchAsync(deleteCommand, token);
            return Ok();
        }

        [HttpGet("universities")]
        public async Task<IEnumerable<NewUniversitiesDto>> GetUniversities(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            var retVal = await _queryBus.QueryAsync<IList<NewUniversitiesDto>> (query, token);
            return retVal;
        }

        [HttpPost("universities")]
        public async Task<IActionResult> MigrateUniversity([FromBody] MigrateUniversityRequest model,
            CancellationToken token)
        {
            var command = new MigrateUniversityCommand(model.UniversityToKeep, model.UniversityToRemove);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}
