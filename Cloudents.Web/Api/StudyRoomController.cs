using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.StudyRooms;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Enum;
using Cloudents.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    [Authorize]
    public class StudyRoomController : ControllerBase
    {

        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly UserManager<User> _userManager;

        public StudyRoomController(ICommandBus commandBus, UserManager<User> userManager, IQueryBus queryBus)
        {
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
            try
            {
                var tutorId = _userManager.GetLongUserId(User);
                var command = new CreateStudyRoomCommand(tutorId, model.UserId);
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (DuplicateRowException)
            {
                return BadRequest("Already active study room");
            }
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
            var query = new StudyRoomQuery(id, userId);
            var result = await _queryBus.QueryAsync(query, token);


            //TODO: need to add who is the tutor
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }

        //private async Task<StudyRoomDto> GetStudyRoomAsync(Guid id, long userId, CancellationToken token)
        //{
        //    var query = new StudyRoomQuery(id, userId);
        //    var result = await _queryBus.QueryAsync(query, token);
        //    return result;
        //}

        [HttpPost("upload"), AllowAnonymous]
        public async Task<IActionResult> UploadAsync(IFormFile file,
            [FromHeader(Name = "referer")] Uri referer,
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
        public async Task<IEnumerable<UserStudyRoomDto>> GetUserLobbyStudyRooms(CancellationToken token)
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

            var url = Url.RouteUrl("roomCallback", new
            {
                id
            }, "https");

            var uri = new Uri(url);
            if (configuration.IsDevelopment())
            {
                var uriBuilder = new UriBuilder(url) { Host = "3c814e9d.ngrok.io", Port = 443 };
                uri = uriBuilder.Uri;
            }


            var command = new CreateStudyRoomSessionCommand(id, configuration.IsProduction(), userId, uri);
            await _commandBus.DispatchAsync(command, token);

            return Ok();


        }

        [HttpPost("roomCallback", Name = "roomCallback"), ApiExplorerSettings(IgnoreApi = true), AllowAnonymous]
        public async Task<IActionResult> TwilioCallBackAsync([FromQuery]Guid id,
            [FromServices] TelemetryClient client,
            [FromForm] TwilioWebHookRequest request, CancellationToken token)
        {
            client.Context.Session.Id = id.ToString();
            client.TrackEvent($"Room Status {id}",
                request.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).ToDictionary
                (
                    propInfo => propInfo.Name,
                    propInfo => propInfo.GetValue(request, null)?.ToString()

                ));
            if (request.RoomStatus == "completed")
            {
                var command = new EndStudyRoomSessionTwilioCommand(id, request.RoomName);
                await _commandBus.DispatchAsync(command, token);
            }
            return Ok();
        }

        /// <summary>
        /// End Tutoring Session
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("{id:guid}/end")]
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

        [HttpPost("review")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewRequest model,
            [FromServices] UserManager<User> userManager,
            CancellationToken token)
        {
            var userId = userManager.GetLongUserId(User);

            var command = new AddTutorReviewCommand(model.RoomId, model.Review, model.Rate, userId);
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

        [HttpPost("NoCard")]
        public async Task<IActionResult> NoCardAsync(NoCardRequest model,
            [FromServices]  IHubContext<StudyRoomHub> studyRoomHubContext, CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User);
            user.PaymentExists = PaymentStatus.Later;
            await _userManager.UpdateAsync(user);
            var message = new SignalRTransportType(SignalRType.User,
                SignalREventAction.PaymentReceived, new object());

            await studyRoomHubContext.Clients.Group(model.StudyRoomId.ToString()).SendAsync(SbHub.MethodName, message, token);
            return Ok();
        }

        //[HttpPost("Money")]
        //public async Task<IActionResult> PayMeCallbackAsync([FromServices] IPayment payment,
        //    CancellationToken token)
        //{
        //    var result = await payment.TransferPaymentAsync("MPL15546-31186SKB-53ES24ZG-WGVCBKO2",
        //        "BUYER155-6007037T-PTSXP1TO-AZ5ULFC9", 100, token);
        //    if (result.StatusCode == 0)
        //    {
        //        return Ok();
        //    }

        //    return BadRequest();
        //}



    }
}