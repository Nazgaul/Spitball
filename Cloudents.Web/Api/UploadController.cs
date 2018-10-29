using Cloudents.Core.Entities.Db;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
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
    [Produces("application/json")]
    [Authorize]
    public class UploadController : Controller
    {
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;
        private readonly IBlobProvider<DocumentContainer> _documentBlobProvider;

        private readonly string[] _supportedImages = { ".jpg", ".png", ".gif", ".jpeg", ".bmp" };


        public UploadController(IBlobProvider<QuestionAnswerContainer> blobProvider, IBlobProvider<DocumentContainer> documentBlobProvider)
        {
            _blobProvider = blobProvider;
            _documentBlobProvider = documentBlobProvider;
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
        public async Task<ActionResult<UploadResponse>> Upload([FromForm] UploadRequest2 model, CancellationToken token)
        {
            var tempData = TempData.Get<TempData>($"update-{model.session_id}");
            // tempData.Indexes = tempData.Indexes ?? new List<long>();
            var index = (int)(model.start_offset / UploadInnerResponse.BlockSize);
            //tempData.Indexes.Add(model.start_offset);
            await _documentBlobProvider.UploadBlockFileAsync(tempData.BlobName, model.Chunk.OpenReadStream(),
                index, token);

            TempData.Put($"update-{model.session_id}", tempData);
            return new UploadResponse();
        }

        [HttpPost("file")]
        public async Task<UploadResponse> Upload([FromBody] UploadRequest model, CancellationToken token)
        {
            if (model.Phase == UploadPhase.Start)
            {
                var response = new UploadResponse(Guid.NewGuid());

                var tempData = new TempData
                {
                    Name = model.Name,
                    Size = model.Size,
                    BlobName = $"{response.Data.SessionId}-{model.Name}{Path.GetExtension(model.Name)}"
                };
                TempData.Put($"update-{response.Data.SessionId}", tempData);
                return response;
            }
            var tempData2 = TempData.Get<TempData>($"update-{model.SessionId}");
            TempData.Remove($"update-{model.SessionId}");

            var indexes = new List<int>();
            for (double i = 0; i < tempData2.Size; i += UploadInnerResponse.BlockSize)
            {
                indexes.Add((int)(i / UploadInnerResponse.BlockSize));
            }
            await _documentBlobProvider.CommitBlockListAsync(tempData2.BlobName, indexes, token);
            return new UploadResponse();
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
        internal const double BlockSize = 3.5e+6;

        public UploadInnerResponse(Guid sessionId)
        {
            SessionId = sessionId;
        }

        [JsonProperty("session_id")]
        public Guid SessionId { get; set; }

        [JsonProperty("end_offset")]
        public double EndOffset => BlockSize;
    }

    public class UploadRequest
    {
        public UploadPhase Phase { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string Name { get; set; }

        [JsonProperty("session_id")]
        public Guid SessionId { get; set; }
    }

    public class TempData
    {
        public string Name { get; set; }

        public string BlobName { get; set; }

        public double Size { get; set; }
        //public IList<long> Indexes { get; set; }
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
        public Guid session_id { get; set; }
        public long start_offset { get; set; }

        public IFormFile Chunk { get; set; }
    }
}