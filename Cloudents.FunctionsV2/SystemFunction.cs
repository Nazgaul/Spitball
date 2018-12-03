using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Cloudents.FunctionsV2.System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
                await operation.DoOperationAsync((dynamic) message, binder, token);
            }
        }


        [FunctionName("SystemFunctionServiceBus")]
        public static async Task Run2(
            [ServiceBusTrigger("background2","default",Connection = "AzureWebJobsServiceBus")]Message receivedMessage,
            [Inject] ILifetimeScope lifetimeScope,
            IBinder binder,
            ILogger log,
            CancellationToken token)
        {

            var messageBodyType =
                Type.GetType(receivedMessage.UserProperties["messageType"].ToString());
            var json = Encoding.UTF8.GetString(receivedMessage.Body);
            var message = JsonConvert.DeserializeObject(json, messageBodyType);

            //log.LogInformation($"Got message {queueMsg}");
            //var message = JsonConvert.DeserializeObject<ISystemQueueMessage>(queueMsg, new JsonSerializerSettings()
            //{
            //    TypeNameHandling = TypeNameHandling.All
            //});

            var handlerType =
                typeof(ISystemOperation<>).MakeGenericType(message.GetType());
            using (var child = lifetimeScope.BeginLifetimeScope())
            {
                dynamic operation = child.Resolve(handlerType);
                await operation.DoOperationAsync((dynamic)message, binder, token);
            }
        }
    }
}
