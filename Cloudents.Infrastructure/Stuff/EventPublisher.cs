using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Stuff
{

    public class EventPublisher : IEventPublisher
    {
        private readonly ILifetimeScope _container;
    
        public EventPublisher(ILifetimeScope subscriptionService)
        {
            _container = subscriptionService;
        }

     

        public async Task PublishAsync(IEvent eventMessage, CancellationToken token)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventMessage.GetType());
            var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);

            
            using (var child = _container.BeginLifetimeScope())
            {
                if (child.Resolve(handlerCollectionType) is IEnumerable eventHandlers)
                    foreach (dynamic handler in eventHandlers)
                    {
                       await handler.HandleAsync((dynamic) eventMessage, token);
                    }

            }
        }
    }
}