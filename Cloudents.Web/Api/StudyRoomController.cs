﻿using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.StudyRooms;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    [Authorize]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Return to client")]
    public class StudyRoomController : ControllerBase
    {

        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly IStringLocalizer<StudyRoomController> _localizer;
        private readonly UserManager<User> _userManager;

        public StudyRoomController(ICommandBus commandBus, UserManager<User> userManager,
            IQueryBus queryBus, IStringLocalizer<StudyRoomController> localizer)
        {
            _commandBus = commandBus;
            _userManager = userManager;
            _queryBus = queryBus;
            _localizer = localizer;
        }

        /// <summary>
        /// Create study room between tutor and student for many sessions - happens in chat
        /// </summary>
        /// <param name="model"></param>
        /// <param name="client">Ignore</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CreateStudyRoomCommandResult>> CreateStudyRoomAsync(CreateStudyRoomRequest model,
            [FromServices] TelemetryClient client,
            CancellationToken token)
        {
            var tutorId = _userManager.GetLongUserId(User);
            try
            {
                var chatTextMessage = _localizer["StudyRoomCreatedChatMessage", model.Name];
                var command = new CreateStudyRoomCommand(tutorId, model.UserId,
                    chatTextMessage, model.Name, model.Price, model.Date, model.Type);
                var result = await _commandBus.DispatchAsync<CreateStudyRoomCommand, CreateStudyRoomCommandResult>(command, token);
                return result;
            }
            catch (DuplicateRowException)
            {
                return Conflict("Already active study room");
            }
            catch (InvalidOperationException e)
            {
                client.TrackException(e, new Dictionary<string, string>()
                {
                    ["UserId"] = model.UserId.ToString(),
                    ["tutorId"] = tutorId.ToString()
                });
                return BadRequest();
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteStudyRoomAsync(Guid id, CancellationToken token)
        {
            try
            {
                var userId = _userManager.GetLongUserId(User);
                var command = new DeleteStudyRoomCommand(id, userId);
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError("error", "Only tutor can delete");
                return BadRequest(ModelState);
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError("error", "Study room with session");
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Get Study Room data and sessionId if opened
        /// </summary>
        /// <param name="id"></param>
        /// <param name="urlBuilder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]


        public async Task<ActionResult<StudyRoomDto>> GetStudyRoomAsync(Guid id,
            [FromServices] IUrlBuilder urlBuilder, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            try
            {
                await _commandBus.DispatchAsync(new EnterStudyRoomCommand(id, userId), default);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            var query = new StudyRoomQuery(id, userId);
            var result = await _queryBus.QueryAsync(query, token);


            //TODO: need to add who is the tutor
            if (result == null)
            {
                return NotFound();
            }
            result.TutorImage = urlBuilder.BuildUserImageEndpoint(result.TutorId, result.TutorImage);
            return result;
        }



        [HttpPost("upload"), AllowAnonymous]
        public async Task<IActionResult> UploadAsync([Required] IFormFile file,
            [FromServices] IDocumentDirectoryBlobProvider blobProvider,
            CancellationToken token)
        {
            var fileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
            await blobProvider
                .UploadStreamAsync(fileName, file.OpenReadStream(), file.ContentType, TimeSpan.FromSeconds(60 * 24), token);

            var uri = blobProvider.GetBlobUrl(fileName);
            var link = blobProvider.GeneratePreviewLink(uri, TimeSpan.FromDays(1));

            return Ok(new
            {
                link
            });

        }

        /// <summary>
        /// Get study rooms data of user - used in study room url
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<UserStudyRoomDto>> GetStudyRoomsAsync(
             CancellationToken token)
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
        public async Task<CreateStudyRoomSessionResponse> CreateStudyRoomSessionAsync([FromRoute] Guid id,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new CreateStudyRoomSessionCommand(id, userId);
            var result = await _commandBus.DispatchAsync<CreateStudyRoomSessionCommand, CreateStudyRoomSessionCommandResult>(command, token);
            return new CreateStudyRoomSessionResponse(result.JwtToken);

        }



        /// <summary>
        /// End Tutoring Session
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("{id:guid}/end")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> EndSessionAsync(Guid id, CancellationToken token)

        {
            try
            {
                var userId = _userManager.GetLongUserId(User);
                var command = new EndStudyRoomSessionCommand(id, userId);
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }


        [HttpPost("{id:guid}/Video")]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        [RequestSizeLimit(209715200)]
        public IActionResult UploadStudyRoomVideo(Guid id,
            IFormFile file,
            CancellationToken token)
        {
            if (file is null)
            {
                return BadRequest();
            }
            //var userId = _userManager.GetLongUserId(User);
            //await using var stream = file.OpenReadStream();
            //var command = new UploadStudyRoomVideoCommand(id, userId, stream);
            //await _commandBus.DispatchAsync(command, token);

            return Ok();
        }


        [HttpPost("review")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CreateReviewAsync([FromBody] ReviewRequest model,
            [FromServices] UserManager<User> userManager,
            CancellationToken token)
        {
            var userId = userManager.GetLongUserId(User);
            if (string.IsNullOrEmpty(model.Review))
            {
                var i = Math.Floor(model.Rate);
                if (!_localizer[$"Review-{i}"].ResourceNotFound)
                {
                    model.Review = _localizer[$"Review-{i}"].Value;
                }

            }


            var command = new AddTutorReviewCommand(model.RoomId, model.Review, model.Rate, userId);
            try
            {
                await _commandBus.DispatchAsync(command, token);
            }
            catch (DuplicateRowException)
            {
                return BadRequest();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}