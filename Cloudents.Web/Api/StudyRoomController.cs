using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.StudyRooms;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    [Authorize]
    public class StudyRoomController : ControllerBase
    {
        private readonly IVideoProvider _videoProvider;
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly UserManager<RegularUser> _userManager;

        public const string CookieName = "studyRoomId";


        public StudyRoomController(IVideoProvider videoProvider,
            ICommandBus commandBus, UserManager<RegularUser> userManager, IQueryBus queryBus)
        {
            _videoProvider = videoProvider;
            _commandBus = commandBus;
            _userManager = userManager;
            _queryBus = queryBus;
        }

        /// <summary>
        /// Create study room between tutor and student for many sessions - happens in chat
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateStudyRoomAsync(CreateStudyRoomRequest model, CancellationToken token)
        {
            var tutorId = _userManager.GetLongUserId(User);
            var command = new CreateStudyRoomCommand(tutorId, model.UserId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        /// <summary>
        /// Get Study Room data and sessionId if opened
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<StudyRoomDto>> GetStudyRoomAsync(Guid id, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var result = await GetStudyRoomAsync(id, userId, token);
            //TODO: need to add who is the tutor
            if (result == null)
            {
                return NotFound();
            }
            Response.Cookies.Append(CookieName, id.ToString(), new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,

            });
            return result;
        }

        private async Task<StudyRoomDto> GetStudyRoomAsync(Guid id, long userId, CancellationToken token)
        {
            var query = new StudyRoomQuery(id, userId);
            var result = await _queryBus.QueryAsync(query, token);
            return result;
        }

        /// <summary>
        /// Get study rooms data of user - used in study room url
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<UserStudyRoomDto>> GetUserStudyRooms(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserStudyRoomQuery(userId);
            return await _queryBus.QueryAsync(query, token);
        }


        /// <summary>
        /// Start a session
        /// </summary>
        /// <returns></returns>
        [HttpPost("{id:guid}/enter")]
        public async Task<IActionResult> CreateAsync([FromRoute] Guid id,
            [FromServices] IHostingEnvironment configuration,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var result = await GetStudyRoomAsync(id, userId, token);
            var session = result.SessionId;
            if (string.IsNullOrEmpty(result.SessionId))
            {

                session = $"{id}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                await _videoProvider.CreateRoomAsync(session, configuration.IsProduction());
                var command = new CreateStudyRoomSessionCommand(id, session, userId);
                await _commandBus.DispatchAsync(command, token);

            }
            var jwtToken = await _videoProvider.ConnectToRoomAsync(session, userId.ToString(), true);
            return Ok(new
            {
                jwtToken
            });
           
        }

        /// <summary>
        /// End Tutoring Session
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("{id:guid}/end")]
        public async Task<IActionResult> EndSessionAsync(Guid id,CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new EndStudyRoomSessionCommand(id,  userId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPost("review")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewRequest model,
            [FromServices] UserManager<RegularUser> userManager,
            CancellationToken token)
        {
            var userId = userManager.GetLongUserId(User);
            if (userId == model.Tutor)
            {
                return BadRequest();
            }

            var command = new AddTutorReviewCommand(model.RoomId, model.Review, model.Rate, model.Tutor, userId);
            try
            {
                await _commandBus.DispatchAsync(command, token);
            }
            catch (DuplicateRowException)
            {
                return BadRequest();
            }
            return Ok();
        }



    }
}