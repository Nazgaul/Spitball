using System.Diagnostics.CodeAnalysis;
using System.Threading;
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
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoke from outside")]
        public static async Task RunAsync(
            [BlobTrigger("spitball-user/profile/{id}/{filename}.{extension}")]CloudBlockBlob myBlob,
            string id,
            [Inject] ICognitiveService cognitiveService,
            ILogger log, CancellationToken token)
        {
            await myBlob.FetchAttributesAsync();
            if (myBlob.Metadata.TryGetValue("face", out _))
            {
                //already process the file
                return;
            }
            log.LogInformation($"Processing {id}");
            await using var sr = await myBlob.OpenReadAsync();
            var result = await cognitiveService.DetectCenterFaceAsync(sr, token);
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
