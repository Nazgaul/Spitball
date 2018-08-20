using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Event;

namespace Cloudents.Core.Interfaces
{
    public interface IEventMessage
    {

    }

    public interface IHaveEvent
    {
        IList<IEventMessage> Events { get; }
    }

    public interface IEventHandler<in T> where T : IEventMessage
    {
        Task HandleAsync(T eventMessage, CancellationToken token);
    }

    //public interface ISubscriptionService
    //{
    //    IEnumerable<IConsumer<T>> GetSubscriptions<T>(); //where T : BaseEvent;
    //}

    //public class EventSubscriptions : ISubscriptionService
    //{


    //    private readonly ILifetimeScope _container;

    //    public EventSubscriptions(ILifetimeScope container)
    //    {
    //        _container = container;
    //    }

    //    public IEnumerable<IConsumer<T>> GetSubscriptions<T>() //where T : BaseEvent
    //    {
    //        //var listType = typeof(IConsumer<>);
    //        //var constructedListType = listType.MakeGenericType(ev.GetType());
    //        //var enumerableType = typeof(IEnumerable<>);
    //        //var resultType = enumerableType.MakeGenericType(constructedListType);

    //        //var enumerable =  _container.Resolve(resultType) as IEnumerable;


    //        //foreach (var t in enumerable)
    //        //{
    //        //    yield return (IConsumer<T>)t;
    //        //}
    //        _container.G
    //        return _container.Resolve<IEnumerable<IConsumer<T>>>();
    //    }
    //}

    public interface IEventPublisher
    {
        Task PublishAsync<T>(T eventMessage, CancellationToken token) where T : IEventMessage;
    }


    public class EventPublisher : IEventPublisher
    {
        // private readonly ISubscriptionService _subscriptionService;
        private readonly ILifetimeScope _container;

        public EventPublisher(ILifetimeScope subscriptionService)
        {
            _container = subscriptionService;
        }

        public async Task PublishAsync<T>(T eventMessage, CancellationToken token) where T : IEventMessage
        {


            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventMessage.GetType());
            var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);

            if (_container.Resolve(handlerCollectionType) is IEnumerable eventHandlers)
                foreach (var handler in (dynamic)eventHandlers)
                {
                    await handler.HandleAsync(eventMessage, token);
                }

            //IEnumerable<IConsumer<T>> GetSubscriptions<T>() 

            // var subscriptions = _subscriptionService.GetSubscriptions<T>();

           // var tasks = subscriptions.Select(s => PublishToConsumerAsync(s, eventMessage, token));
            //await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        //private static async Task PublishToConsumerAsync<T>(IConsumer<T> x, T eventMessage, CancellationToken token)// where T : BaseEvent
        //{
        //    try
        //    {
        //        await x.HandleAsync(eventMessage, token);
        //    }
        //    catch (Exception e)
        //    {
        //        //log and handle internally
        //    }
        //    finally
        //    {
        //        var instance = x as IDisposable;
        //        if (instance != null)
        //        {
        //            instance.Dispose();
        //        }
        //    }
        //}
    }
}