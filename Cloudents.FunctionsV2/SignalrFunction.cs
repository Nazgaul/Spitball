using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Cloudents.FunctionsV2
{
    public static class SignalrFunction
    {
        [FunctionName("SignalRMessage")]
        public static async Task Run2(
            [ServiceBusTrigger("signalr", Connection = "AzureWebJobsServiceBus")]
            string receivedMessage,
            IDictionary<string, object> userProperties,
            [SignalR(HubName = "SbHub")] IAsyncCollector<SignalRMessage> outMessage,
            ILogger log,
            CancellationToken token
        )
        {
            var msg = JObject.Parse(receivedMessage);
            log.LogInformation($"Receive signalr message {msg}");





            var p = new SignalRMessage
            {
                Target = "Message",
                Arguments = new object[] { msg },

            };
            if (userProperties.TryGetValue("userId", out var userId))
            {
                p.UserId = userId?.ToString();
            }
            if (userProperties.TryGetValue("group", out var group))
            {
                p.GroupName = @group?.ToString();
            }


            await outMessage.AddAsync(p, token);
        }
    }
}