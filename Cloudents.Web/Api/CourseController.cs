﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Course api controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseSearch _courseProvider;
        private readonly ICommandBus _commandBus;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="courseProvider"></param>
        /// <param name="commandBus"></param>
        public CourseController(ICourseSearch courseProvider, ICommandBus commandBus)
        {
            _courseProvider = courseProvider;
            _commandBus = commandBus;
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
        public async Task<CoursesResponse> GetAsync([FromQuery]  CourseRequest model, CancellationToken token)
        {
            var result = await _courseProvider.SearchAsync(model.Term, model.UniversityId.GetValueOrDefault(), token).ConfigureAwait(false);
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
        public async Task<CoursesCreateResponse> CreateAcademicBoxAsync([FromBody]CreateCourseRequest model, CancellationToken token)
        {
            var command = new CreateCourseCommand(model.CourseName, model.University);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return new CoursesCreateResponse
            {
                Id = command.Id
            };
        }
    }
}
