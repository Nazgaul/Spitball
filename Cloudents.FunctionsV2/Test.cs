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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Cloudents.FunctionsV2
{
    public static class Test
    {
        //[FunctionName("RemoveDuplicatePendingQuestion")]
        //public static async Task Run(
        //    [TimerTrigger("0 */20 * * * *", RunOnStartup = true)]TimerInfo timer,
        //    [SignalR(HubName = "SbHub")]IAsyncCollector<JObject> signalRMessages,
        //    CancellationToken token
        //)
        //{
        //    var z = new SignalRTransportType(SignalRType.Question, SignalRAction.Add, new QuestionFeedDto(
        //        1,
        //        QuestionSubject.ComputerScience,
        //        1000,
        //        "RAAAAAAAMMMMMM",
        //        0,
        //        0,
        //        null,
        //        DateTime.UtcNow,
        //        QuestionColor.Blue,
        //        false,
        //        new CultureInfo("en")));


        //    var p = new SignalRMessage
        //    {
        //        Target = "Message",
        //        Arguments = new object[] { z },
        //        UserId = "159423"
        //    };

        //    var settings = new JsonSerializerSettings()
        //    {

        //        NullValueHandling = NullValueHandling.Ignore,
        //        DateTimeZoneHandling = DateTimeZoneHandling.Utc,
        //        ContractResolver = new CamelCasePropertyNamesContractResolver()
        //    };
        //    settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
        //    var x = JsonConvert.SerializeObject(p, settings);
           
        //    await signalRMessages.AddAsync(JObject.Parse(x));



        //}
    }
}
