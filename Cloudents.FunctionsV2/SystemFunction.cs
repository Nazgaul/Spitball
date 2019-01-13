using Autofac;
using Cloudents.FunctionsV2.System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Newtonsoft.Json.Linq;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class SystemFunction
    {
        [FunctionName("SystemFunction")]
        public static async Task Run([QueueTrigger(QueueName.BackgroundQueueName)]string queueMsg,
            [Inject] ILifetimeScope lifetimeScope,
            IBinder binder,
            ILogger log,
            CancellationToken token)
        {
            log.LogInformation($"Got message {queueMsg}");
            var message = JsonConvert.DeserializeObject<ISystemQueueMessage>(queueMsg, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });

            var handlerType =
                typeof(ISystemOperation<>).MakeGenericType(message.GetType());
            using (var child = lifetimeScope.BeginLifetimeScope())
            {
                dynamic operation = child.Resolve(handlerType);
                await operation.DoOperationAsync((dynamic)message, binder, token);
            }
        }

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
            if (userProperties.TryGetValue("userId",out var userId))
            {
                p.UserId = userId?.ToString();
            }
            if (userProperties.TryGetValue("group", out var group))
            {
                p.GroupName = group?.ToString();
            }


            await outMessage.AddAsync(p, token);
        }
    }
}
