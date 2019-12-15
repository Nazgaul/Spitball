using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.FunctionsV2.FileProcessor;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class mediaServiceOutput
    {
        [FunctionName("media-service-event")]
        public static async Task Run(
            [QueueTrigger("media-service")] string message,
            [Inject] VideoProcessor videoProvider,
            IBinder binder,
            CancellationToken token)
        {

            dynamic json = JToken.Parse(message);
            foreach (var output in json.data.outputs)
            {
                string label = output.label;
                string assetName = output.assetName;

                if (label == AssetType.Thumbnail.ToString())
                {
                    var id = long.Parse(RegEx.NumberExtractor.Match(assetName).Value);
                    await videoProvider.MoveImageAsync(id, binder, token);
                }

                if (label == AssetType.Short.ToString())
                {
                    var id = long.Parse(RegEx.NumberExtractor.Match(assetName).Value);
                    await videoProvider.CreateLocatorAsync(id, token);
                }

                if (label == AssetType.Long.ToString())
                {
                    var id = long.Parse(RegEx.NumberExtractor.Match(assetName).Value);
                    await videoProvider.UpdateDurationAsync(id, binder, token);
                }

                if (label == AssetType.StudyRoom.ToString())
                {
                    await videoProvider.MoveStudyRoomVideoAsync(assetName, binder, token);
                }
            }

        }
    }
}
