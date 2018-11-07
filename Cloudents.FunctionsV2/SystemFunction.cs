using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Cloudents.FunctionsV2.Di;
using Cloudents.FunctionsV2.System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Rest.Serialization;
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
            var message = JsonConvert.DeserializeObject<BaseSystemMessage>(queueMsg, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });
            var operation = lifetimeScope.ResolveKeyed<ISystemOperation>(message.Type);
            await operation.DoOperationAsync(message, binder, token);
        }
    }
}
