using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Query;
using Cloudents.Query.Admin;
using Cloudents.Query.Query.Admin;
using Dapper;
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
        private readonly IDapperRepository _dapperRepository;

        public AdminCourseController(IQueryBus queryBus, ICommandBus commandBus, IDapperRepository dapperRepository)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _dapperRepository = dapperRepository;
        }

        //TODO: Fix this and make it work in proper CQRS architecture
        [HttpPost("migrate")]
        public async Task<IActionResult> MigrateCourse([FromBody] MigrateCourseRequest model,
            CancellationToken token)
        {
            const string update = @"
                            update sb.Document
                            set CourseName = @newId
                            where CourseName = @oldId;

                            update sb.Question
                            set CourseId = @newId
                            where CourseId = @oldId;

                            update sb.UsersCourses 
                            set CourseId = @newId
                            where CourseId = @oldId
                            and UserId not in (select UserId from sb.UsersCourses where CourseId = @newId);
                            
                            delete from sb.UsersCourses where CourseId = @oldId;

                            delete from sb.Course where [Name] = @oldId;";


            using (var connection = _dapperRepository.OpenConnection())
            {
                await connection.ExecuteAsync(update, new
                {
                    newId = model.CourseToKeep,
                    oldId = model.CourseToRemove
                });
            }

            /*var command = new MigrateCourseCommand(model.CourseToKeep, model.CourseToRemove);
            var deleteCommand = new DeleteCourseCommand(model.CourseToRemove);
            await _commandBus.DispatchAsync(command, token);
            await _commandBus.DispatchAsync(deleteCommand, token);*/

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
        //[Authorize(Policy = Policy.IsraelUser)]
        public async Task<CoursesResponse> GetAsync([FromQuery(Name = "course")]string course,
            CancellationToken token)
        {
            var query = new AdminCourseSearchQuery(0, course, 0, User.GetCountryClaim());
            //var query = new CourseSearchWithTermQuery(0, course, 0);
            var result = await _queryBus.QueryAsync(query, token);
            return new CoursesResponse
            {
                Courses = result
            };
        }

        [HttpGet("newCourses")]
        [Authorize]
        public async Task<IEnumerable<PendingCoursesDto>> GetNewCourses([FromQuery]CoursesRequest model
                , CancellationToken token)
        {

            var query = new AdminCoursesQuery(model.Language, model.State.GetValueOrDefault(ItemState.Pending),
                User.GetCountryClaim(),
                model.Filter);
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }

      


        [HttpPost("approve")]
        public async Task<IActionResult> ApproveCourse([FromBody] ApproveCourseRequest model,
                CancellationToken token)
        {
            var command = new ApproveCourseCommand(model.Course, model.Subject, model.SchoolType);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }


        //TODO: Fix this and make it work in proper CQRS architecture
        [HttpPost("rename")]
        public async Task<IActionResult> RenameCourse([FromBody] RenameCourseRequest model,
                CancellationToken token)
        {
            const string update = @"
                            insert into sb.Course (Name, Count, State) 
                            values(@newId, (select [Count] from sb.Course where Name = @oldId), 'Ok' );

                            update sb.Document
                            set CourseName = @newId
                            where CourseName = @oldId;

                            update sb.Question
                            set CourseId = @newId
                            where CourseId = @oldId;

                            update sb.UsersCourses 
                            set CourseId = @newId
                            where CourseId = @oldId;
                            delete from sb.Course where [Name] = @oldId;";


            using (var connection = _dapperRepository.OpenConnection())
            {
                await connection.ExecuteAsync(update, new
                {
                    newId = model.NewName,
                    oldId = model.OldName
                });
            }
            /*var command = new RenameCourseCommand(model.OldName, model.NewName);
            await _commandBus.DispatchAsync(command, token);*/
            return Ok();
        }


        [HttpGet("subject")]
        [Authorize]
        public async Task<IEnumerable<string>> GetSubjects(CancellationToken token)
        {
            var query = new SubjectsQuery();
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }


        [HttpDelete("{name}")]
        [Authorize]
        public async Task<IActionResult> DeleteCourse(string name,
                CancellationToken token)
        {
            var command = new DeleteCourseCommand(name);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}
