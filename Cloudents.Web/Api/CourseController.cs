using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.Entities;
using Cloudents.Query;
using Cloudents.Query.Courses;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Courses;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Interfaces;
using Cloudents.Query.Users;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Course api controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController, Authorize]
    [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting", Justification = "Api")]

    public class CourseController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly UserManager<User> _userManager;
        private readonly IUrlBuilder _urlBuilder;
        private readonly ICommandBus _commandBus;

        public CourseController(IQueryBus queryBus, UserManager<User> userManager, IUrlBuilder urlBuilder, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _userManager = userManager;
            _urlBuilder = urlBuilder;
            _commandBus = commandBus;
        }

        [HttpGet("{id:long}"), AllowAnonymous]
        public async Task<ActionResult<CourseDetailDto?>> GetCourseByIdAsync([FromRoute] long id, CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new CourseByIdQuery(id, userId);
            var result = await _queryBus.QueryAsync(query, token);
            if (result == null)
            {
                return NotFound();
            }
            result.TutorImage = _urlBuilder.BuildUserImageEndpoint(result.TutorId, result.TutorImage);
            result.Image = _urlBuilder.BuildCourseThumbnailEndPoint(result.Id);
            return result;
        }

        [HttpGet("{id:long}/edit")]
        [Authorize(Policy = "Tutor")]
        public async Task<ActionResult<CourseDetailEditDto>> GetCourseDetailForUpdateAsync([FromRoute] long id, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new CourseByIdEditQuery(id, userId);
            var result = await _queryBus.QueryAsync(query, token);
            if (result == null)
            {
                return NotFound();
            }
            result.Image = _urlBuilder.BuildCourseThumbnailEndPoint(result.Id);
            return result;
        }

        [HttpPut("{id:long}")]
        [Authorize(Policy = "Tutor")]
        public async Task<IActionResult> UpdateCourseAsync([FromRoute] long id, [FromBody] CreateCourseRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);

            var command = new UpdateCourseCommand(userId, model.Name, (int)model.Price,
                (int?)model.SubscriptionPrice, model.Description, model.Image,
                model.StudyRooms.Select(s => new UpdateCourseCommand.UpdateLiveStudyRoomCommand(s.Name, s.Date)),
                model.Documents.Select(
                    s => new UpdateCourseCommand.UpdateDocumentCommand(
                        s.Id, s.BlobName, s.Name, s.Visible)),
                model.IsPublish, id);

            await _commandBus.DispatchAsync(command, token);


            return Ok();
        }


        [HttpPost]
        [Authorize(Policy = "Tutor")]
        public async Task<IActionResult> CreateCourseAsync([FromBody] CreateCourseRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);

            var command = new CreateCourseCommand(userId, model.Name, (int)model.Price,
                (int?)model.SubscriptionPrice, model.Description, model.Image,
                model.StudyRooms.Select(s => new CreateCourseCommand.CreateLiveStudyRoomCommand(s.Name, s.Date)),
                model.Documents.Select(
                    s => new CreateCourseCommand.CreateDocumentCommand(s.BlobName, s.Name, s.Visible)),
                model.IsPublish);

            await _commandBus.DispatchAsync(command, token);


            return Ok();
        }


        [HttpPost("{id:long}/enroll")]
        public async Task EnrollUpcomingEventAsync([FromRoute] long id, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new CourseEnrollCommand(userId, id);
            await _commandBus.DispatchAsync(command, token);

        }

        [HttpGet]
        [Authorize(Policy = "Tutor")]
        public async Task<IEnumerable<UserCoursesDto>> GetMyCoursesAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserCoursesByIdQuery(userId);
            var result = await _queryBus.QueryAsync(query, token);

            return result.Select(s =>
            {
                s.Image = _urlBuilder.BuildCourseThumbnailEndPoint(s.Id);
                return s;
            });
        }

        /// <summary>
        /// Perform course search - we can't put cache because the user can re-enter the page
        /// </summary>
        /// <param name="request">params</param>
        /// <param name="token"></param>
        /// <returns>list of courses filter by input</returns>
        [HttpGet("search")]
        [AllowAnonymous]

        public async Task<CoursesResponse> GetAsync(
           [FromQuery] CourseSearchRequest request,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);


            var query = new CourseSearchQuery(userId, request.Term);
            var temp = await _queryBus.QueryAsync(query, token);


            return new CoursesResponse(temp);
        }
    }
}
