using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Cloudents.Core.Exceptions;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"),ApiController]
    [Authorize]
    public class StudyRoomController : ControllerBase
    {
        private readonly IQueueProvider _queueProvider;
        private readonly IVideoProvider _videoProvider;
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly UserManager<RegularUser> _userManager;

        public StudyRoomController(IQueueProvider queueProvider, IVideoProvider videoProvider,
            ICommandBus commandBus, UserManager<RegularUser> userManager, IQueryBus queryBus)
        {
            _queueProvider = queueProvider;
            _videoProvider = videoProvider;
            _commandBus = commandBus;
            _userManager = userManager;
            _queryBus = queryBus;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudyRoomAsync(CreateStudyRoomRequest model, CancellationToken token)
        {
            var tutorId = _userManager.GetLongUserId(User);
            var command = new CreateStudyRoomCommand(tutorId, model.UserId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpGet("id:guid")]
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

        [HttpGet]
        public async Task<IEnumerable<UserStudyRoomDto>> GetUserStudyRooms(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserStudyRoomQuery(userId);
            return await _queryBus.QueryAsync(query, token);
        }


        /// <summary>
        /// Generate room
        /// </summary>
        /// <returns></returns>
        [HttpPost("id:guid/start")]
        public async Task<IActionResult> CreateAsync([FromRoute] Guid id,
            [FromServices] IHostingEnvironment configuration,
            CancellationToken token)
        {
            var roomName = $"{id}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            var t1 = _videoProvider.CreateRoomAsync(roomName, configuration.IsProduction());
            var t2 = _queueProvider.InsertMessageAsync(new EndTutoringSessionMessage(roomName), TimeSpan.FromMinutes(90), token);
            await Task.WhenAll(t1, t2);
            return Ok(new
            {
                name = roomName
            });
        }

        [HttpGet("id:guid/join")]
        public async Task<IActionResult> ConnectAsync(string roomName)
        {
            var user = _userManager.GetUserId(User);
            //TODO: need to distinguish tutor from not.
            var token = await _videoProvider.ConnectToRoomAsync(roomName, user,true);
            return Ok(new
            {
                token
            }
            );
        }

        //[HttpPost("upload")]
        //public async Task<IActionResult> UploadAsync(IFormFile file,
        //    [FromServices] IDocumentDirectoryBlobProvider blobProvider,
        //    CancellationToken token)
        //{
        //    var fileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
        //    await blobProvider
        //        .UploadStreamAsync(fileName, file.OpenReadStream(), file.ContentType, TimeSpan.FromSeconds(60 * 24), token);

        //    var uri = blobProvider.GetBlobUrl(fileName);
        //    var link = blobProvider.GeneratePreviewLink(uri, TimeSpan.FromDays(1));

        //    return Ok(new
        //    {
        //        link
        //    });

        //}

        //[HttpPost("document")]
        //public async Task<IActionResult> CreateOnlineDocument([FromBody] OnlineDocumentRequest model, CancellationToken token)
        //{
        //    var url = await _googleDocument.CreateOnlineDocAsync(model.Name, token);
        //    return Ok(new
        //    {
        //        link = url
        //    });
        //}

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
            
            var command = new AddTutorReviewCommand(model.Review, model.Rate, model.Tutor, userId);
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