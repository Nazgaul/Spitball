using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.FunctionsV2.System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Cloudents.FunctionsV2
{
    public static class Test
    {
        //[FunctionName("RemoveDuplicatePendingQuestion")]
        //public static async Task Run(
        //    [ServiceBusTrigger("topic1","xxx",Connection = "AzureWebJobsServiceBus")] string name,
        //    ILogger log,
        //    CancellationToken token
        //)
        //{
        //    log.LogInformation(name);
        //}
    }
}
