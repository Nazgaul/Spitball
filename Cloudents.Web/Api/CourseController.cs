﻿using Cloudents.Command;
using Cloudents.Command.Courses;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Query;
using Cloudents.Query.Courses;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Models;
using Cloudents.Web.Binders;

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
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public CourseController(IQueryBus queryBus, ICommandBus commandBus, UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Perform course search - we can't put cache because the user can re-enter the page
        /// </summary>
        /// <param name="request">params</param>
        /// <param name="profile"></param>
        /// <param name="token"></param>
        /// <returns>list of courses filter by input</returns>
        [Route("search")]
        [HttpGet, AllowAnonymous]

        public async Task<IEnumerable<CourseDto>> GetAsync(
           [FromQuery] CourseSearchRequest request,
           [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            if (!string.IsNullOrEmpty(request.Term))
            {
                var query = new CourseSearchWithTermQuery(userId, request.Term, request.Page, profile.CountryRegion);
                return await _queryBus.QueryAsync(query, token);
            }
            else
            {
                var query = new CourseSearchQuery(userId, request.Page, profile.CountryRegion);
                return await _queryBus.QueryAsync(query, token);
            }

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CreateCoursesAsync([FromBody] SetCourseRequest model, CancellationToken token)
        {
            try
            {
                var userId = _userManager.GetLongUserId(User);
                var command = new CreateCourseCommand(userId, model.Name);
                await _commandBus.DispatchAsync(command, token);
                var user = await _userManager.GetUserAsync(User);
                await _signInManager.RefreshSignInAsync(user);
                return Ok(model);
            }
            catch (DuplicateRowException)
            {
                return Conflict();
            }
        }

        [HttpPost("teach")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> TeachCoursesAsync([FromBody] SetCourseRequest model, CancellationToken token)
        {
            try
            {
                var userId = _userManager.GetLongUserId(User);
                var command = new TeachCourseCommand(userId, model.Name);
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (InvalidOperationException)
            {
                ModelState.AddModelError("x", "Not such course");
                return BadRequest();
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCoursesAsync([FromQuery, Required]string name, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new UserRemoveCourseCommand(userId, name);
            await _commandBus.DispatchAsync(command, token);
            var user = await _userManager.GetUserAsync(User);
            return Ok();
        }

        [HttpGet("subject"), AllowAnonymous]
        public async Task<SubjectDto> GetSubjectAsync([FromQuery, Required] string courseName,
            CancellationToken token)
        {
            var query = new CourseSubjectQuery(courseName);
            var result = await _queryBus.QueryAsync(query, token);
            return result;

        }
    }
}
