using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;

namespace Cloudents.Core.Interfaces
{
    //public interface IEventMessage
    //{

    //}

    public interface IConsumer<T>
    {
        Task HandleAsync(T eventMessage, CancellationToken token);
    }

    public interface ISubscriptionService
    {
        IEnumerable<IConsumer<T>> GetSubscriptions<T>();
    }

    public class EventSubscriptions : ISubscriptionService
    {

        public static void Add<T>(ContainerBuilder builder)
        {
            var consumerType = typeof(T);

            consumerType.GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConsumer<>))
                .ToList()
                .ForEach(x => builder.RegisterType(x));
        }
        private readonly ILifetimeScope _container;

        public EventSubscriptions(ILifetimeScope container)
        {
            _container = container;
        }

        public IEnumerable<IConsumer<T>> GetSubscriptions<T>()
        {
            return _container.Resolve<IEnumerable<IConsumer<T>>>();
        }
    }

    public interface IEventPublisher
    {
        Task PublishAsync<T>(T eventMessage, CancellationToken token);
    }


    public class EventPublisher : IEventPublisher
    {
        private readonly ISubscriptionService _subscriptionService;

        public EventPublisher(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public async Task PublishAsync<T>(T eventMessage, CancellationToken token)
        {
            var subscriptions = _subscriptionService.GetSubscriptions<T>();

            var tasks = subscriptions.Select(s => PublishToConsumerAsync(s, eventMessage, token));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private static async Task PublishToConsumerAsync<T>(IConsumer<T> x, T eventMessage, CancellationToken token)
        {
            try
            {
                await x.HandleAsync(eventMessage, token);
            }
            catch (Exception e)
            {
                //log and handle internally
            }
            finally
            {
                var instance = x as IDisposable;
                if (instance != null)
                {
                    instance.Dispose();
                }
            }
        }
    }
}