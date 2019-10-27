using Autofac;
using Cloudents.FunctionsV2.System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
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
                var binderTyped = new TypedParameter(typeof(IBinder), binder);
                dynamic operation = child.Resolve(handlerType, binderTyped);
                await operation.DoOperationAsync((dynamic)message, binder, token);
            }
        }
    }
}
