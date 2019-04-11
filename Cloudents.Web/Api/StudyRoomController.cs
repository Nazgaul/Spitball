using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
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

        public StudyRoomController(IVideoProvider videoProvider,
            ICommandBus commandBus, UserManager<RegularUser> userManager, IQueryBus queryBus)
        {
            _videoProvider = videoProvider;
            _commandBus = commandBus;
            _userManager = userManager;
            _queryBus = queryBus;
        }

        /// <summary>
        /// Create study room between tutor and student for many sessions
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
            //TODO: signalr
            return Ok();
        }

        /// <summary>
        /// Get Study Room data and sessionId if opened
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<StudyRoomDto>> GetStudyRoomAsync(Guid id, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new StudyRoomQuery(id, userId);
            var result = await _queryBus.QueryAsync(query, token);
            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        /// <summary>
        /// Get study rooms data of user
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
        [HttpPost("{id:guid}/start")]
        public async Task<IActionResult> CreateAsync([FromRoute] Guid id,
            [FromServices] IHostingEnvironment configuration,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var session = $"{id}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            await _videoProvider.CreateRoomAsync(session, configuration.IsProduction());
            var command = new CreateStudyRoomSessionCommand(id, session, userId);
            await _commandBus.DispatchAsync(command, token);

            //TODO signalr
            // var t1 = _videoProvider.CreateRoomAsync(roomName, configuration.IsProduction());
            //  var t2 = _queueProvider.InsertMessageAsync(new EndTutoringSessionMessage(roomName), TimeSpan.FromMinutes(90), token);
            // await Task.WhenAll(t1, t2);
            return Ok(new
            {
                session
            });
        }

        /// <summary>
        /// Join to an open session
        /// </summary>
        /// <param name="id"></param>
        /// <param name="session"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("{id:guid}/join")]
        public async Task<IActionResult> ConnectToSessionAsync(Guid id,
           [FromBody] string session
           )
        {
            var user = _userManager.GetUserId(User);
            var token = await _videoProvider.ConnectToRoomAsync(session, user, true);
            return Ok(new
            {
                token
            });
        }


        /// <summary>
        /// End Open Session
        /// </summary>
        /// <param name="id"></param>
        /// <param name="session"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("{id:guid}/end")]
        public async Task<IActionResult> EndSessionAsync(Guid id,
            [FromBody] string session
        )
        {
            //TODO need to validate
            await _videoProvider.CloseRoomAsync(session);
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