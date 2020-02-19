using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class ProfileFaceDetectFunction
    {
        [FunctionName("ProfileFaceDetectFunction")]
        public static async Task Run([BlobTrigger("spitball-user/profile/{id}/{filename}.{extension}")]CloudBlockBlob myBlob,
            string id,
            [Inject] ICognitiveService cognitiveService,
            ILogger log)
        {
            await myBlob.FetchAttributesAsync();
            if (myBlob.Metadata.TryGetValue("face", out _))
            {
                return;
            }
            log.LogInformation($"Processing {id}");
            await using var sr = await myBlob.OpenReadAsync();
            var result = await cognitiveService.DetectCenterFaceAsync(sr);
            if (result is null)
            {
                return;
            }
            myBlob.Metadata["face"] = $"{result.Value.X},{result.Value.Y}";
            await myBlob.SetMetadataAsync();
            log.LogInformation($"Finish Processing {id}");
        }
    }
}
