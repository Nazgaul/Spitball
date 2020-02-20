using Autofac;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.FunctionsV2.Operations;
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

            var handlerType = typeof(ISystemOperation<>).MakeGenericType(message.GetType());
            var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);

            using (var child = lifetimeScope.BeginLifetimeScope())
            {

                if (child.Resolve(handlerCollectionType) is IEnumerable handlersCollection)
                {
                    foreach (var handler in handlersCollection)
                    {
                        try
                        {
                            dynamic operation = child.Resolve(handler.GetType());
                            await operation.DoOperationAsync((dynamic)message, binder, token);
                        }
                        catch (Exception e)
                        {
                            log.LogInformation(e.Message);
                        }
                    }
                }
            }
        }
    }
}
