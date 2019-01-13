//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.SignalRService;
//using Microsoft.Extensions.Logging;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.FunctionsV2
//{
//    public static class Test
//    {
//        [FunctionName("XXXXX")]
//        public static async Task Run(
//            [TimerTrigger("0 * * * * *", RunOnStartup = true)]TimerInfo myTimer,
//            [SignalR(HubName = "SbHub")] IAsyncCollector<SignalRMessage> outMessage,
//            ILogger log,
//            CancellationToken token
//        )
//        {
//            await outMessage.AddAsync(new SignalRMessage()
//            {
//                // GroupName = "country-il",
//                Target = "Message",
//                Arguments = new object[] { " ram  ram" },
//            }, token);
//            await outMessage.AddAsync(new SignalRMessage()
//            {
//                GroupName = "country-us",
//                Target = "Message",
//                Arguments = new object[] { "us ram us ram" },
//            }, token);


//            await outMessage.AddAsync(new SignalRMessage()
//            {
//                GroupName = "country-il",
//                Target = "Message",
//                Arguments = new object[] { "il ram il ram" },
//            }, token);

//            await outMessage.AddAsync(new SignalRMessage()
//            {
//                GroupName = "Ram",
//                Target = "Message",
//                Arguments = new object[] { "RAM RRAM" },
//            }, token);


//            await outMessage.AddAsync(new SignalRMessage()
//            {
//                UserId = "638",
//                Target = "Message",
//                Arguments = new object[] { "RAM RRAM RRAM RRAM RRAM" },
//            }, token);



//        }
//    }
//}
