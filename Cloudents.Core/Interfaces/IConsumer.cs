using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Cloudents.Core.Interfaces
{
    public interface IConsumer<T>
    {
        void Handle(T eventMessage);
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
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IConsumer<>))
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
        void Publish<T>(T eventMessage);
    }


    public class EventPublisher : IEventPublisher
    {
        private readonly ISubscriptionService _subscriptionService;

        public EventPublisher(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public void Publish<T>(T eventMessage)
        {
            var subscriptions = _subscriptionService.GetSubscriptions<T>();
            subscriptions.ToList().ForEach(x => PublishToConsumer(x, eventMessage));
        }

        private static void PublishToConsumer<T>(IConsumer<T> x, T eventMessage)
        {
            try
            {
                x.Handle(eventMessage);
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