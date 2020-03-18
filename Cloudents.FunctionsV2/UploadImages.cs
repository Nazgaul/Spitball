using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.FunctionsV2.Binders;
using Cloudents.FunctionsV2.FileProcessor;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Cloudents.FunctionsV2
{
    public static class UploadImages
    {

        [FunctionName("ProcessPowerPoint")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            CancellationToken token)
        {
            var model = context.GetInput<PowerPointOrchestrationInput>();

            foreach (var image in model.Images)
            {
                await context.CallActivityAsync("UploadImagesToStorage", new PowerPointActivityInput
                {
                    Id = model.Id,
                    Image = image
                });
            }
          
        }

        [FunctionName("UploadImagesToStorage")]
        public static async Task SayHello([ActivityTrigger] PowerPointActivityInput imageResult,
            [Blob("spitball-files/files/{imageResult.Id}")]CloudBlobDirectory directory,
            [HttpClientFactory] HttpClient httpClient,
            ILogger log)
        {
            var previewBlob = directory.GetBlockBlobReference($"preview-{imageResult.Image.PageNumber - 1}.jpg");
            previewBlob.Properties.ContentType = "image/jpeg";
            await using var imageStream = await httpClient.GetStreamAsync(imageResult.Image.URL);
            using var input = Image.Load<Rgba32>(imageStream);
            await using var blobWriteStream = await previewBlob.OpenWriteAsync();
            input.SaveAsJpeg(blobWriteStream);

        }

       
    }
}