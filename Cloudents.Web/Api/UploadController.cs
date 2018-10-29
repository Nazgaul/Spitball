using Cloudents.Core.Entities.Db;
using Cloudents.Core.Storage;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    //DO NOT ADD API CONTROLLER - UPLOAD WILL NOT WORK
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public class UploadController : ControllerBase
    {
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;
        private readonly string[] _supportedImages = { ".jpg", ".png", ".gif", ".jpeg", ".bmp" };


        public UploadController(IBlobProvider<QuestionAnswerContainer> blobProvider)
        {
            _blobProvider = blobProvider;
        }

        // GET
        [HttpPost("ask")]
        public async Task<UploadAskFileResponse> UploadFileAsync(UploadFileRequest model,
            [FromServices] UserManager<User> userManager,
            CancellationToken token)
        {
            var userId = userManager.GetUserId(User);

            var fileNames = new List<string>();
            foreach (var formFile in model.File)
            {
                if (!formFile.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("not an image");
                }

                var extension = Path.GetExtension(formFile.FileName);

                if (!_supportedImages.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("not an image");
                }

                using (var sr = formFile.OpenReadStream())
                {
                    Image.FromStream(sr);
                    var fileName = $"{userId}.{Guid.NewGuid()}.{formFile.FileName}";
                    await _blobProvider
                        .UploadStreamAsync(fileName, sr, formFile.ContentType, false, 60 * 24, token);

                    fileNames.Add(fileName);
                }
            }
            return new UploadAskFileResponse(fileNames);
        }



        [HttpPost("file"), FormContentType]
        public async Task<ActionResult<UploadResponse>> Upload([FromForm] UploadRequest2 model)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1));
            return new UploadResponse();
        }

        [HttpPost("file")]
        public async Task<ActionResult<UploadResponse>> Upload([FromBody] UploadRequest model)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1));
            if (model.Phase == UploadPhase.Start)
            {
                var response = new UploadResponse(Guid.NewGuid());
                return response;
            }
            return Ok();
        }

    }

    public class UploadResponse
    {
        public UploadResponse(Guid sessionId)
        {
            Data = new UploadInnerResponse(sessionId);
        }

        public UploadResponse()
        {
            
        }

        public string Status => "success";

        public UploadInnerResponse Data { get; set; }
    }

    public class UploadInnerResponse
    {
        public UploadInnerResponse(Guid sessionId)
        {
            SessionId = sessionId;
        }

        [JsonProperty("session_id")]
        public Guid SessionId { get; set; }

        [JsonProperty("session_id")]
        public double EndOffset => 4e+6;
    }

    public class UploadRequest
    {
        public UploadPhase Phase { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string Name { get; set; }
    }

    public enum UploadPhase
    {
        Start,
        Upload,
        Finish
    }

    public class UploadRequest2
    {
        public UploadPhase Phase { get; set; }
        [JsonProperty("session_id")]
        public Guid SessionId { get; set; }
        [JsonProperty("start_offset")]

        public long StartOffset { get; set; }

        public IFormFile Chunk { get; set; }
    }
}