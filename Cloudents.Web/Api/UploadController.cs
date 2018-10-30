﻿using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Localization;
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
        private readonly IStringLocalizer<UploadController> _localizer;



        public UploadController(IBlobProvider<QuestionAnswerContainer> blobProvider, IBlobProvider<DocumentContainer> documentBlobProvider, IStringLocalizer<UploadController> localizer)
        {
            _blobProvider = blobProvider;
            _documentBlobProvider = documentBlobProvider;
            _localizer = localizer;
        }

        // GET
        [HttpPost("ask")]
        public async Task<UploadAskFileResponse> UploadFileAsync(UploadAskFileRequest model,
            [FromServices] UserManager<User> userManager,
            CancellationToken token)
        {
            string[] supportedImages = { ".jpg", ".png", ".gif", ".jpeg", ".bmp" };
            
            var userId = userManager.GetUserId(User);

            var fileNames = new List<string>();
            foreach (var formFile in model.File)
            {
                if (!formFile.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("not an image");
                }

                var extension = Path.GetExtension(formFile.FileName);

                if (!supportedImages.Contains(extension, StringComparer.OrdinalIgnoreCase))
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

            await Task.Delay(TimeSpan.FromSeconds(5));
            // tempData.Indexes = tempData.Indexes ?? new List<long>();
            var index = (int)(model.start_offset / UploadInnerResponse.BlockSize);
            //tempData.Indexes.Add(model.start_offset);
            await _documentBlobProvider.UploadBlockFileAsync(tempData.BlobName, model.Chunk.OpenReadStream(),
                index, token);

            TempData.Put($"update-{model.session_id}", tempData);
            return new UploadResponse();
        }

        [HttpPost("file")]
        public async Task<ActionResult<UploadResponse>> Upload([FromBody] UploadRequest model, CancellationToken token)
        {

            string[] supportedFiles = { "doc", "docx", "xls", "xlsx", "PDF", "png", "jpg", "ppt", "ppt" };

            var extension = Path.GetExtension(model.Name).TrimStart('.');

            if (!supportedFiles.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.Name), _localizer["Upload"]);
                return BadRequest(ModelState);
            }

            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(model.MimeType, out contentType))
            {
                ModelState.AddModelError(nameof(model.Name), _localizer["Upload"]);
                return BadRequest(ModelState);
            }
            

            if (model.Phase == UploadPhase.Start)
            {
                var response = new UploadResponse(Guid.NewGuid());

                var tempData = new TempData
                {
                    Name = model.Name,
                    Size = model.Size,
                    BlobName = $"{response.Data.SessionId}-{model.Name}"
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
            return new UploadResponse(tempData2.BlobName);
        }


        [HttpPost("dropbox")]
        public async Task<UploadResponse> UploadDropBox([FromBody] DropBoxRequest model,
            [FromServices] IRestClient client,
            CancellationToken token)
        {
            var (stream, _) = await client.DownloadStreamAsync(model.Link, token);
            var blobName = $"{Guid.NewGuid()}-{model.Name}";
            await _documentBlobProvider.UploadStreamAsync(blobName, stream, token: token);

            return new UploadResponse(blobName);
        }

    }


}