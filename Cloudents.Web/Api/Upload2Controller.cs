using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Api
{

    public abstract class UploadControllerBase : ControllerBase
    {

        protected readonly IBlobProvider _blobProvider;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        protected UploadControllerBase(IBlobProvider blobProvider, ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            _blobProvider = blobProvider;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
        }

        // GET: api/<controller>
        [HttpPost("upload"), FormContentType, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<UploadStartResponse>> BatchUploadAsync(
            [FromForm] UploadRequestForm model,
            CancellationToken token)
        {
            var tempDataProvider = _tempDataDictionaryFactory.GetTempData(HttpContext);
            var tempData = tempDataProvider.Get<TempData>($"update-{model.SessionId}");
            var index = (int)(model.StartOffset / UploadInnerResponse.BlockSize);
            await _blobProvider.UploadBlockFileAsync(tempData.BlobName, model.Chunk.OpenReadStream(),
                index, token);

            tempDataProvider.Put($"update-{model.SessionId}", tempData);
            return new UploadStartResponse();

        }



        [HttpPost("upload"), ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<UploadStartResponse>> Upload([FromBody] UploadRequestBase model,
            CancellationToken token)
        {
            var result = await Upload(model as dynamic, token);
            return result;
        }

        [NonAction]
        protected virtual string[] GetSupportedExtensions()
        {
            return new[]{ "doc",
            "docx", "xls",
            "xlsx", "PDF",
            "png", "jpg",
            "jpeg",
            "ppt", "pptx","tiff","tif","bmp" };
        }


        private Task<UploadStartResponse> Upload(UploadRequestStart model, CancellationToken token)
        {


            var extension = Path.GetExtension(model.Name)?.TrimStart('.');

            if (!GetSupportedExtensions().Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException();
                //ModelState.AddModelError(nameof(model.Name), _localizer["Upload"]);
                //return BadRequest(ModelState);
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



        protected virtual string BlobFileName(Guid sessionId, string name)
        {
            Regex rgx = new Regex(@"[^\x00-\x7F]+|\s+");
            name = rgx.Replace(name, string.Empty);
            if (name.StartsWith('.'))
            {
                name = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + name;
            }
            return $"file-{sessionId}-{name.Replace("/", string.Empty)}";
        }

        private async Task<UploadStartResponse> Upload(UploadRequestFinish model, CancellationToken token)
        {
            var tempDataProvider = _tempDataDictionaryFactory.GetTempData(this.HttpContext);
            var tempData2 = tempDataProvider.Get<TempData>($"update-{model.SessionId}");

            tempDataProvider.Remove($"update-{model.SessionId}");

            var indexes = new List<int>();
            for (double i = 0; i < tempData2.Size; i += UploadInnerResponse.BlockSize)
            {
                indexes.Add((int)(i / UploadInnerResponse.BlockSize));
            }

            await _blobProvider.CommitBlockListAsync(tempData2.BlobName, tempData2.MimeType, tempData2.Name, indexes, token);
            var result = tempData2.BlobName;
            await FinishUploadAsync(model, result, token);
            return new UploadStartResponse(result);
        }

        public abstract Task FinishUploadAsync(UploadRequestFinish model, string blobName, CancellationToken token);
    }
}
