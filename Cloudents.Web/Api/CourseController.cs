﻿using System;
using Cloudents.Command;
using Cloudents.Command.Courses;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Framework;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Web.Identity;

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
        /// <param name="request">params</param>
        /// <param name="token"></param>
        /// <returns>list of courses filter by input</returns>
        [Route("search")]
        [HttpGet]
        [ResponseCache(Duration = TimeConst.Hour,
            Location = ResponseCacheLocation.Client,
            VaryByQueryKeys = new[] { "*" })]
        public async Task<CoursesResponse> GetAsync(
           [FromQuery] CourseSearchRequest request,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new CourseSearchQuery(userId, request.Term, request.Page);
            var result = await _queryBus.QueryAsync(query, token);
            return new CoursesResponse
            {
                Courses = result
            };
        }
       

        [HttpPost("set")]
        public async Task<IActionResult> SetCoursesAsync([FromBody] SetCourseRequest[] model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new UserJoinCoursesCommand(model.Select(s => s.Name), userId);
            await _commandBus.DispatchAsync(command, token);
            var user = await _userManager.GetUserAsync(User);
            await _signInManager.RefreshSignInAsync(user);
            return Ok(model);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateCoursesAsync([FromBody] SetCourseRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new CreateCourseCommand(userId, model.Name);
            await _commandBus.DispatchAsync(command, token);
            var user = await _userManager.GetUserAsync(User);
            await _signInManager.RefreshSignInAsync(user);
            return Ok(model);
        }

        [HttpPost("teach")]
        public async Task<IActionResult> TeachCoursesAsync([FromBody] SetCourseRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new TeachCourseCommand(userId, model.Name);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCoursesAsync([FromQuery, Required]string name, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new UserRemoveCourseCommand(userId, name);
            await _commandBus.DispatchAsync(command, token);
            var user = await _userManager.GetUserAsync(User);
            await _signInManager.RefreshSignInAsync(user);
            return Ok();
        }
    }
}
