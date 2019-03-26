using System;
using Cloudents.Core.Storage;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Api
{
   
    public abstract class UploadControllerBase : ControllerBase
    {


        // GET: api/<controller>
        [HttpPost("upload"), FormContentType, ApiExplorerSettings(IgnoreApi = true)]
        public  async Task<ActionResult<UploadResponse>> BatchUploadAsync(
            [FromForm] UploadRequest2 model,
            [FromServices] UploadService.Factory factory,
            [FromServices] IChatDirectoryBlobProvider blobProvider,
            CancellationToken token)
        {

            var service = factory.Invoke(blobProvider);
            return await service.UploadBatchAsync(model, token);
        }

        [HttpPost("upload", Order = 0), StartUploading, ApiExplorerSettings(IgnoreApi = true)]
        public  UploadResponse StartUpload([FromBody] UploadRequest model,
            [FromServices] UploadService.Factory factory,
            [FromServices] IChatDirectoryBlobProvider blobProvider,
            CancellationToken token)
        {
            var service = factory.Invoke(blobProvider);
            return service.StartUpload(model);
        }

        [HttpPost("upload", Order = 1), ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<UploadResponse>> FinishUpload([FromBody] UploadRequest model,
            [FromServices] UploadService.Factory factory,
            [FromServices] IChatDirectoryBlobProvider blobProvider,
            CancellationToken token)
        {
            var service = factory.Invoke(blobProvider);
            var result =  await service.FinishUploadAsync(model, token);
            await FinishUploadAsync(model);
            return result;
        }

        public abstract Task FinishUploadAsync(UploadRequest model);
    }
}
