using Autofac;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.FunctionsV2.Operations;
using Cloudents.Infrastructure;
using NHibernate;
using NHibernate.Linq;
using SendGrid.Helpers.Mail;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class SystemFunction
    {



        [FunctionName("SystemFunction")]
        public static async Task Run([QueueTrigger(QueueName.BackgroundQueueName)] string queueMsg,
            [Inject] ILifetimeScope lifetimeScope,
            IBinder binder,
            ILogger log,
            CancellationToken token)
        {
            //log.LogInformation($"Got message {queueMsg}");

            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                //Converters.Add(new EnumerationConverter<Country>());
            };
            settings.Converters.Add(new EnumerationConverter<Country>());

            var message = JsonConvert.DeserializeObject<ISystemQueueMessage>(queueMsg, settings);

            var handlerType = typeof(ISystemOperation<>).MakeGenericType(message.GetType());
            var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);

            using var child = lifetimeScope.BeginLifetimeScope();
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
