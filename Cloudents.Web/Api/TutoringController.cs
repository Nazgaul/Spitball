using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;


namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class TutoringController : ControllerBase
    {
        private readonly IQueueProvider _queueProvider;
        private readonly IVideoProvider _videoProvider;
        private readonly IGoogleDocument _googleDocument;
        public TutoringController(IQueueProvider queueProvider, IVideoProvider videoProvider, IGoogleDocument googleDocument)
        {
            _queueProvider = queueProvider;
            _videoProvider = videoProvider;
            _googleDocument = googleDocument;
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
            [FromServices] IDocumentDirectoryBlobProvider blobProvider,
            CancellationToken token)
        {
            var fileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
            await blobProvider
                .UploadStreamAsync(fileName, file.OpenReadStream(), file.ContentType, false, 60 * 24, token);

            var uri = blobProvider.GetBlobUrl(fileName);
            var link = blobProvider.GeneratePreviewLink(uri, TimeSpan.FromDays(1));

            return Ok(new
            {
                link
            });

        }

        [HttpPost("document")]
        public async Task<IActionResult> CreateOnlineDocument([FromBody] OnlineDocumentRequest model, CancellationToken token)
        {
            var url = await _googleDocument.CreateOnlineDocAsync(model.Name, token);
            return Ok(new
            {
                link = url
            });
        }
    }
}