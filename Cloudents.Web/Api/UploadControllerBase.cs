using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Web.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Api
{

    public abstract class UploadControllerBase : ControllerBase
    {

        protected readonly IBlobProvider BlobProvider;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly IStringLocalizer<UploadControllerBase> _localizer;

        protected UploadControllerBase(IBlobProvider blobProvider, ITempDataDictionaryFactory tempDataDictionaryFactory, IStringLocalizer<UploadControllerBase> localizer)
        {
            BlobProvider = blobProvider;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _localizer = localizer;
        }

        // GET: api/<controller>
        [HttpPost("upload"), FormContentType, ApiExplorerSettings(IgnoreApi = true),Authorize]
        public async Task<ActionResult<UploadStartResponse>> BatchUploadAsync(
            [FromForm] UploadRequestForm model,
            CancellationToken token)
        {
            var tempDataProvider = _tempDataDictionaryFactory.GetTempData(HttpContext);
            var tempData = tempDataProvider.Get<TempData>($"update-{model.SessionId}");
            if (tempData == null)
            {
                ModelState.AddModelError("error","bad upload");
                return BadRequest(ModelState);
            }
            var index = (int)(model.StartOffset / UploadInnerResponse.BlockSize);
            await BlobProvider.UploadBlockFileAsync(tempData.BlobName, model.Chunk.OpenReadStream(),
                index, token);

            tempDataProvider.Put($"update-{model.SessionId}", tempData);
            return new UploadStartResponse();

        }



        [HttpPost("upload"), ApiExplorerSettings(IgnoreApi = true), Authorize]
        public async Task<ActionResult<UploadStartResponse>> Upload([FromBody] UploadRequestBase model,
            CancellationToken token)
        {
            try
            {
                var result = await Upload(model as dynamic, token);
                return result;
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError(nameof(model), string.Format(_localizer["Upload"], string.Join(", ", GetSupportedExtensions())));
                return BadRequest(ModelState);

            }
        }

        [NonAction]
        protected virtual IEnumerable<string> GetSupportedExtensions()
        {
          return  FormatDocumentExtensions.GetFormats();
        }


        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "need the same method signature")]
        private Task<UploadStartResponse> Upload(UploadRequestStart model, CancellationToken token)
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
            var result = tempData2.BlobName;
            await FinishUploadAsync(model, result, token);
            return new UploadStartResponse(result);
        }

        public abstract Task FinishUploadAsync(UploadRequestFinish model, string blobName, CancellationToken token);
    }
}
