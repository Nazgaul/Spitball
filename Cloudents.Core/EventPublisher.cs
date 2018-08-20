using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core
{
    public class EventPublisher : IEventPublisher
    {
        // private readonly ISubscriptionService _subscriptionService;
        private readonly ILifetimeScope _container;

        public EventPublisher(ILifetimeScope subscriptionService)
        {
            _container = subscriptionService;
        }

        public async Task PublishAsync<T>(T eventMessage, CancellationToken token) where T : IEvent
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventMessage.GetType());
            var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);

            if (_container.Resolve(handlerCollectionType) is IEnumerable eventHandlers)
                foreach (dynamic handler in eventHandlers)
                {
                    await handler.HandleAsync((dynamic)eventMessage, token);
                }
        }
    }
}