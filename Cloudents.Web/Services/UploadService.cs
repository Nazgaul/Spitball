using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Services
{
    public class UploadService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly IBlobProvider _blobProvider;
        private static readonly Random Random = new Random();

        public UploadService(IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataDictionaryFactory, IBlobProvider blobProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _blobProvider = blobProvider;
        }

        public delegate UploadService Factory(IBlobProvider blobProvider);

        public UploadStartResponse StartUpload(UploadRequestStart model)
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
            var tempDataProvider = _tempDataDictionaryFactory.GetTempData(_httpContextAccessor.HttpContext);
            tempDataProvider.Put($"update-{response.Data.SessionId}", tempData);
            return response;
        }

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

        public async Task<UploadStartResponse> UploadBatchAsync(UploadRequestForm model, CancellationToken token)
        {

            var tempDataProvider = _tempDataDictionaryFactory.GetTempData(_httpContextAccessor.HttpContext);
            var tempData = tempDataProvider.Get<TempData>($"update-{model.SessionId}");
            // var blobProvider = blobProviderIndex[type];
            var index = (int)(model.StartOffset / UploadInnerResponse.BlockSize);
            await _blobProvider.UploadBlockFileAsync(tempData.BlobName, model.Chunk.OpenReadStream(),
                index, token);

            tempDataProvider.Put($"update-{model.SessionId}", tempData);
            return new UploadStartResponse();
        }

        public async Task<string> FinishUploadAsync(UploadRequestFinish model, CancellationToken token)
        {
            var tempDataProvider = _tempDataDictionaryFactory.GetTempData(_httpContextAccessor.HttpContext);
            var tempData2 = tempDataProvider.Get<TempData>($"update-{model.SessionId}");

            tempDataProvider.Remove($"update-{model.SessionId}");

            var indexes = new List<int>();
            for (double i = 0; i < tempData2.Size; i += UploadInnerResponse.BlockSize)
            {
                indexes.Add((int)(i / UploadInnerResponse.BlockSize));
            }

            await _blobProvider.CommitBlockListAsync(tempData2.BlobName, tempData2.MimeType, indexes, token);
            return tempData2.BlobName;
        }
    }
}