using Autofac.Features.Indexed;
using Cloudents.Command;
using Cloudents.Command.Command;
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

namespace Cloudents.Web.Api
{
    //DO NOT ADD API CONTROLLER - UPLOAD WILL NOT WORK
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Produces("application/json")]
    [Authorize]
    public class UploadController : Controller
    {
        private readonly IQuestionsDirectoryBlobProvider _blobProvider;
        private readonly IStringLocalizer<UploadController> _localizer;



        public UploadController(IQuestionsDirectoryBlobProvider blobProvider,
            IStringLocalizer<UploadController> localizer)
        {
            _blobProvider = blobProvider;
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
                        .UploadStreamAsync(fileName, sr, formFile.ContentType, TimeSpan.FromSeconds(60 * 24), token);

                    fileNames.Add(fileName);
                }
            }
            return new UploadAskFileResponse(fileNames);
        }



        [HttpPost("file"), FormContentType]
        public async Task<ActionResult<UploadStartResponse>> Upload(
            [FromRoute] StorageContainer type,
            [FromForm] UploadRequestForm model,
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
            return new UploadStartResponse();
        }


        [HttpPost("file", Order = 0), StartUploading]
        public ActionResult<UploadStartResponse> StartUploadAsync(
            [FromBody] UploadRequestStart model)
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


            var response = new UploadStartResponse(Guid.NewGuid());

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

        [HttpPost("file", Order = 1)]
        public async Task<ActionResult<UploadStartResponse>> FinishUploadAsync(
            [FromServices] IDocumentDirectoryBlobProvider documentBlobProvider,
            [FromBody] UploadRequestFinish model,
            CancellationToken token)
        {
            var blobName = await CommitBlobsAsync(documentBlobProvider, model, token);
            return new UploadStartResponse(blobName);
        }

        private async Task<string> CommitBlobsAsync(IBlobProvider documentBlobProvider, UploadRequestFinish model,
            CancellationToken token)
        {
            var tempData2 = TempData.Get<TempData>($"update-{model.SessionId}");

            TempData.Remove($"update-{model.SessionId}");

            var indexes = new List<int>();
            for (double i = 0; i < tempData2.Size; i += UploadInnerResponse.BlockSize)
            {
                indexes.Add((int)(i / UploadInnerResponse.BlockSize));
            }

            await documentBlobProvider.CommitBlockListAsync(tempData2.BlobName, tempData2.MimeType, indexes, token);
            return tempData2.BlobName;
        }


        //[HttpPost("{type:regex(chat)}", Order = 1)]
        //public async Task<ActionResult<UploadStartResponse>> FinishUploadAsync(
        //    [FromServices] IChatDirectoryBlobProvider blobProvider,
        //    [FromServices] ICommandBus bus,
        //    [FromServices] UserManager<RegularUser> userManager,
        //    [FromBody] FinishChatUpload model,
        //    CancellationToken token)
        //{
        //    var blobName = await CommitBlobsAsync(blobProvider, model, token);


        //    var command = new SendMessageCommand(null, userManager.GetLongUserId(User), new[] { model.OtherUser }, null, blobName);
        //    await bus.DispatchAsync(command, token);
        //    return Ok();
        //    //return new UploadResponse(tempData2.BlobName);
        //}

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
        public async Task<UploadStartResponse> UploadDropBox([FromBody] DropBoxRequest model,
            [FromServices] IRestClient client,
            [FromServices] IDocumentDirectoryBlobProvider documentDirectoryBlobProvider,
            CancellationToken token)
        {
            var (stream, _) = await client.DownloadStreamAsync(model.Link, token);
            var blobName = BlobFileName(Guid.NewGuid(), model.Name);
            await documentDirectoryBlobProvider.UploadStreamAsync(blobName, stream, token: token);

            return new UploadStartResponse(blobName);
        }

    }


}