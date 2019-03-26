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
        public  async Task<ActionResult<UploadStartResponse>> BatchUploadAsync(
            [FromForm] UploadRequestForm model,
            [FromServices] UploadService.Factory factory,
            [FromServices] IChatDirectoryBlobProvider blobProvider,
            CancellationToken token)
        {

            var service = factory.Invoke(blobProvider);
            return await service.UploadBatchAsync(model, token);
        }

        //[HttpPost("upload", Order = 0),  ApiExplorerSettings(IgnoreApi = true)]
        //public  UploadStartResponse StartUpload([FromBody] UploadRequestStart model,
        //    [FromServices] UploadService.Factory factory,
        //    [FromServices] IChatDirectoryBlobProvider blobProvider,
        //    CancellationToken token)
        //{
        //    var service = factory.Invoke(blobProvider);
        //    return service.StartUpload(model);
        //}

        [HttpPost("upload"), ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<UploadStartResponse>> FinishUpload([FromBody] UploadRequestBase model,
            [FromServices] UploadService.Factory factory,
            [FromServices] IChatDirectoryBlobProvider blobProvider,
            CancellationToken token)
        {
            var result = await Upload(model as dynamic, factory, blobProvider, token);
            //var service = factory.Invoke(blobProvider);
            //var result =  await service.FinishUploadAsync(model, token);
            //await FinishUploadAsync(model);
            return result;
        }
       
        private Task<UploadStartResponse> Upload(UploadRequestStart model, UploadService.Factory factory, IChatDirectoryBlobProvider blobProvider, CancellationToken token)
        {
            var service = factory.Invoke(blobProvider);
            return Task.FromResult(service.StartUpload(model));
        }

        private async Task<UploadStartResponse> Upload(UploadRequestFinish model, UploadService.Factory factory, IChatDirectoryBlobProvider blobProvider, CancellationToken token)
        {
            var service = factory.Invoke(blobProvider);
            var result = await service.FinishUploadAsync(model, token);
            await FinishUploadAsync(model, result, token);
            return new UploadStartResponse(result); 
        }

        public abstract Task FinishUploadAsync(UploadRequestFinish model,string blobName, CancellationToken token);
    }
}
