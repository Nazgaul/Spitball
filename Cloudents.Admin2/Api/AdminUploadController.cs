using Cloudents.Admin2.Extensions;
using Cloudents.Admin2.Framework;
using Cloudents.Admin2.Models;
using Cloudents.Core;
using Cloudents.Core.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    public class AdminUploadController : ControllerBase
    {
        protected readonly IAdminDirectoryBlobProvider BlobProvider;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        internal const double BlockSize = 3.5e+6;
        public AdminUploadController(IAdminDirectoryBlobProvider blobProvider, ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            BlobProvider = blobProvider;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
        }

        [HttpPost("upload"), FormContentType, ApiExplorerSettings(IgnoreApi = true), Authorize]
        public async Task<ActionResult<UploadStartResponse>> BatchUploadAsync(
            [FromForm] UploadRequestForm model,
            CancellationToken token)
        {
            var tempDataProvider = _tempDataDictionaryFactory.GetTempData(HttpContext);
            var tempData = tempDataProvider.Get<TempData>($"update-{model.SessionId}");
            if (tempData == null)
            {
                ModelState.AddModelError("error", "bad upload");
                return BadRequest(ModelState);
            }
            var index = (int)(model.StartOffset / UploadInnerResponse.BlockSize);
            await BlobProvider.UploadBlockFileAsync(tempData.BlobName, model.Chunk.OpenReadStream(),
                index, token);

            tempDataProvider.Put($"update-{model.SessionId}", tempData);
            return new UploadStartResponse();
        }

        [NonAction]
        private IEnumerable<string> GetSupportedExtensions()
        {
            return FileTypesExtensions.GetFormats();
        }

        private string BlobFileName(Guid sessionId, string name)
        {
            Regex rgx = new Regex(@"[^\x00-\x7F]+|\s+");
            name = rgx.Replace(name, string.Empty);
            if (name.StartsWith('.'))
            {
                name = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + name;
            }
            return $"video-{sessionId}-{name.Replace("/", string.Empty)}";
        }


        [HttpPost("upload"), ApiExplorerSettings(IgnoreApi = true), Authorize]
        public Task<UploadStartResponse> Upload(UploadRequestStart model, CancellationToken token)
        {


            var extension = Path.GetExtension(model.Name);

            if (!GetSupportedExtensions().Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException();
            }


            var response = new UploadStartResponse(Guid.NewGuid());

            var tempData = new TempData
            {
                Name = model.Name,
                Size = model.Size,
                BlobName = BlobFileName(response.Data.SessionId, model.Name),
                MimeType = model.MimeType
            };
            var tempDataProvider = _tempDataDictionaryFactory.GetTempData(HttpContext);
            tempDataProvider.Put($"update-{response.Data.SessionId}", tempData);
            return Task.FromResult(response);
        }

        [HttpPost("upload"), ApiExplorerSettings(IgnoreApi = true), Authorize]
        public async Task<Uri> Upload(UploadRequestFinish model, CancellationToken token)
        {
            var tempDataProvider = _tempDataDictionaryFactory.GetTempData(HttpContext);
            var tempData2 = tempDataProvider.Get<TempData>($"update-{model.SessionId}");

            tempDataProvider.Remove($"update-{model.SessionId}");

            var indexes = new List<int>();
            for (double i = 0; i < tempData2.Size; i += UploadInnerResponse.BlockSize)
            {
                indexes.Add((int)(i / UploadInnerResponse.BlockSize));
            }

            //original file name can only have ascii chars. hebrew not supported. remove that
            await BlobProvider.CommitBlockListAsync(tempData2.BlobName, tempData2.MimeType, null, indexes, token);
            return BlobProvider.GetBlobUrl(tempData2.BlobName);
        }


    }

}
