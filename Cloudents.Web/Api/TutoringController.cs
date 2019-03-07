using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;


namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TutoringController : ControllerBase
    {

        private readonly IQueueProvider _queueProvider;
        private readonly IVideoProvider _videoProvider;
       


       

        public TutoringController(IQueueProvider queueProvider, IVideoProvider videoProvider)
        {
            _queueProvider = queueProvider;
            _videoProvider = videoProvider;
        }


        /// <summary>
        /// Generate room
        /// </summary>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(CancellationToken token)
        {
            var roomName = Guid.NewGuid().ToString();
            var t1 = _videoProvider.CreateRoomAsync(roomName);

            var t2 = _queueProvider.InsertMessageAsync(new EndTutoringSessionMessage(roomName), TimeSpan.FromMinutes(90), token);
            await Task.WhenAll(t1, t2);
            //RoomResource.Update(room.UniqueName,RoomResource.RoomStatusEnum.Completed)
            return Ok(new
            {
                name = roomName
            });
        }


        [HttpGet("join")]
        public async Task<IActionResult> ConnectAsync(string roomName, string identityName)
        {
            var token = await _videoProvider.ConnectToRoomAsync(roomName, identityName);
            return Ok(new
            {
                token
            }
            );
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAsync(IFormFile file,
            [FromServices] IBlobProvider<DocumentContainer> blobProvider,
            [FromServices] IBlobProvider blobProvider2,
            CancellationToken token)
        {
            var fileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
            await blobProvider
                .UploadStreamAsync(fileName, file.OpenReadStream(), file.ContentType, false, 60 * 24, token);

            var uri = blobProvider.GetBlobUrl(fileName);
            var link = blobProvider2.GeneratePreviewLink(uri, TimeSpan.FromDays(1));

            return Ok(new
            {
                link
            });

        }
       
    }


    
}