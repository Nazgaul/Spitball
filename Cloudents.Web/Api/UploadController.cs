﻿using Autofac.Features.Indexed;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Cloudents.Web.Api
{
    //DO NOT ADD API CONTROLLER - UPLOAD WILL NOT WORK
    [Route("api/[controller]")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Produces("application/json")]
    [Authorize]
    public class UploadController : Controller
    {
        private readonly IQuestionsDirectoryBlobProvider _blobProvider;
        //private readonly IDocumentDirectoryBlobProvider _documentBlobProvider;
        private readonly IStringLocalizer<UploadController> _localizer;



        public UploadController(IQuestionsDirectoryBlobProvider blobProvider,
            // IDocumentDirectoryBlobProvider documentBlobProvider, 
            IStringLocalizer<UploadController> localizer)
        {
            _blobProvider = blobProvider;
            // _documentBlobProvider = documentBlobProvider;
            _localizer = localizer;
        }

        // GET
        [HttpPost("ask")]
        public async Task<UploadAskFileResponse> UploadFileAsync(UploadAskFileRequest model,
            [FromServices] UserManager<RegularUser> userManager,
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
                    //Image.FromStream(sr);
                    var fileName = $"{userId}.{Guid.NewGuid()}.{formFile.FileName}";
                    await _blobProvider
                        .UploadStreamAsync(fileName, sr, formFile.ContentType, false, 60 * 24, token);

                    fileNames.Add(fileName);
                }
            }
            return new UploadAskFileResponse(fileNames);
        }



        [HttpPost("{type:StorageContainerConstraint}"), FormContentType]
        public async Task<ActionResult<UploadResponse>> Upload(
            [FromRoute] StorageContainer type,
            [FromForm] UploadRequest2 model,
            [FromServices] IIndex<StorageContainer, IBlobProvider> blobProviderIndex,
            CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tempData = TempData.Get<TempData>($"update-{model.SessionId}");
            var blobProvider = blobProviderIndex[type];
            var index = (int)(model.StartOffset / UploadInnerResponse.BlockSize);
            await blobProvider.UploadBlockFileAsync(tempData.BlobName, model.Chunk.OpenReadStream(),
                index, token);

            TempData.Put($"update-{model.SessionId}", tempData);
            return new UploadResponse();
        }


        [HttpPost("{type:StorageContainerConstraint}", Order = 0), StartUploading]
        public ActionResult<UploadResponse> StartUploadAsync(
            [FromBody] UploadRequest model)
        {
            string[] supportedFiles = { "doc",
                "docx", "xls",
                "xlsx", "PDF",
                "png", "jpg",
                "jpeg",
                "ppt", "pptx","tiff","tif","bmp" };

            var extension = Path.GetExtension(model.Name)?.TrimStart('.');

            if (!supportedFiles.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.Name), _localizer["Upload"]);
                return BadRequest(ModelState);
            }


            var response = new UploadResponse(Guid.NewGuid());

            var tempData = new TempData
            {
                Name = model.Name,
                Size = model.Size,
                BlobName = BlobFileName(response.Data.SessionId, model.Name),
                MimeType = model.MimeType
            };
            TempData.Put($"update-{response.Data.SessionId}", tempData);
            return response;
        }

        [HttpPost("{type:regex(file)}", Order = 1)]
        public async Task<ActionResult<UploadResponse>> FinishUploadAsync(
            [FromServices] IDocumentDirectoryBlobProvider documentBlobProvider,
            [FromBody] UploadRequest model,
            CancellationToken token)
        {
            var blobName = await CommitBlobsAsync(documentBlobProvider, model, token);
            return new UploadResponse(blobName);
        }

        private async Task<string> CommitBlobsAsync(IBlobProvider documentBlobProvider, UploadRequest model,
            CancellationToken token)
        {
            var tempData2 = TempData.Get<TempData>($"update-{model.SessionId}");

            TempData.Remove($"update-{model.SessionId}");

            var indexes = new List<int>();
            for (double i = 0; i < tempData2.Size; i += UploadInnerResponse.BlockSize)
            {
                indexes.Add((int) (i / UploadInnerResponse.BlockSize));
            }

            await documentBlobProvider.CommitBlockListAsync(tempData2.BlobName, tempData2.MimeType, indexes, token);
            return tempData2.BlobName;
        }


        [HttpPost("{type:regex(chat)}", Order = 1)]
        public async Task<ActionResult<UploadResponse>> FinishUploadAsync(
            [FromServices] IChatDirectoryBlobProvider blobProvider,
            [FromServices] IHubContext<SbHub> hubContext,
            [FromServices] UserManager<RegularUser> userManager,
            [FromBody] FinishChatUpload model,
            CancellationToken token)
        {
            var blobName = await CommitBlobsAsync(blobProvider, model, token);
            //Command
            await hubContext.Clients.Users(new[]
            {
                userManager.GetUserId(User), model.OtherUser.ToString(),
            }).SendAsync("Chat", new
            {
                file = blobName

            }, cancellationToken: token);
            return Ok();
            //return new UploadResponse(tempData2.BlobName);
        }

        private static readonly Random Random = new Random();

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        private static string BlobFileName(Guid sessionId, string name)
        {
            Regex rgx = new Regex(@"[^\x00-\x7F]+|\s+");
            name = rgx.Replace(name, string.Empty);
            if (name.StartsWith('.'))
            {
                name = RandomString(3) + name;
            }
            return $"file-{sessionId}-{name.Replace("/", string.Empty)}";
        }


        [HttpPost("dropbox")]
        public async Task<UploadResponse> UploadDropBox([FromBody] DropBoxRequest model,
            [FromServices] IRestClient client,
            [FromServices] IDocumentDirectoryBlobProvider documentDirectoryBlobProvider,
            CancellationToken token)
        {
            var (stream, _) = await client.DownloadStreamAsync(model.Link, token);
            var blobName = BlobFileName(Guid.NewGuid(), model.Name);
            await documentDirectoryBlobProvider.UploadStreamAsync(blobName, stream, token: token);

            return new UploadResponse(blobName);
        }

    }


}