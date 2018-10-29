using Cloudents.Core.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System;
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
        public static async Task Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [SignalRConnectionInfo(HubName = "SbHub")]SignalRConnectionInfo connectionInfo,
            [SignalR(HubName = "SbHub")]  IAsyncCollector<SignalRMessage> signalRMessages,
            [Inject] IQueryBus queryBus2,
            ILogger log,
            CancellationToken token)
        {

            var dto = new QuestionDto
            {
                User = new UserDto
                {
                    Id = 638,
                    Name = "yaari.9181",
                    Image = null
                },
                Answers = 0,
                Id = 2827,
                DateTime = DateTime.Parse("2018-10-20T23:09:18.486Z"),
                Files = 0,
                HasCorrectAnswer = false,
                Price = 10,
                Text = "54645 6ryt yfthfhgf hft 54 ryt5656 rytyrtyrty",
                Color = QuestionColor.Default,
                Subject = QuestionSubject.Biology
            };
            var retVal = new SignalRTransportType(SignalRType.Question, SignalRAction.Add, dto);
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,

                //o.PayloadSerializerSettings.Converters.Add(new StringEnumNullUnknownStringConverter { CamelCaseText = true });
            };
            jsonSerializerSettings.Converters.Add(new StringEnumConverter(true));
            var t = JsonConvert.SerializeObject(retVal, jsonSerializerSettings);
            log.LogInformation("Starting Test v2");
            await signalRMessages.AddAsync(
                 new SignalRMessage
                 {
                     Target = "Message",
                     Arguments = new object[] { t }
                 });

            //var command = new UpdateQuestionTimeCommand();
            //await commandBus.DispatchAsync(command, token);
            log.LogInformation($"Test C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
