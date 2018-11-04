using Cloudents.Core.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class Test
    {
        [FunctionName("Test")]
        public static async Task Run(
            [TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo timer,
           [Blob("spitball-files/files/1/text.txt")] string text, IDictionary<string, string> metadata
            //[BlobTrigger("spitball-files/files/{id}/{name}")] Stream myBlob,string id, string name
            )
        {


        }
    }
}
