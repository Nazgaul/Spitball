using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Stuff
{

    //public class EventStore : IEventStore, IEnumerable<IEvent>
    //{
    //    private readonly List<IEvent> _events = new List<IEvent>();


    //    public void Add(IEvent @event)
    //    {
    //        _events.Add(@event);
    //    }

    //    //public IEnumerable<IEvents> Get()
    //    //{
    //    //    return _events.AsReadOnly().ToList();
    //    //    System.Collections.ObjectModel.ReadOnlyCollection<IEvent> t = _events.AsReadOnly();
    //    //    return t;
    //    //}

    //    public IEnumerator<IEvent> GetEnumerator()
    //    {
    //        foreach (var @event in _events.AsReadOnly())
    //        {
    //            yield return @event;
    //        }
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }
    //}




    public class EventPublisher : IEventPublisher
    {
        private readonly ILifetimeScope _container;
    
        public EventPublisher(ILifetimeScope subscriptionService)
        {
            _container = subscriptionService;
        }

        //public async Task PublishAsync<T>(T eventMessage, CancellationToken token) where T : IEvent
        //{
        //    var handlerType = typeof(IEventHandler<>).MakeGenericType(eventMessage.GetType());
        //    var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);


        //    var tasks = new List<Task>();
        //    if (_container.Resolve(handlerCollectionType) is IEnumerable eventHandlers)
        //        foreach (dynamic handler in eventHandlers)
        //        {
        //            var t = handler.HandleAsync((dynamic) eventMessage, token);
        //            tasks.Add(t);
        //        }

        //    await Task.WhenAll(tasks);
        //}

        public async Task PublishAsync(IEvent eventMessage, CancellationToken token)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventMessage.GetType());
            var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);

            
            var tasks = new List<Task>();
            if (_container.Resolve(handlerCollectionType) is IEnumerable eventHandlers)
                foreach (dynamic handler in eventHandlers)
                {
                    var t = handler.HandleAsync((dynamic)eventMessage, token);
                    tasks.Add(t);
                }

            await Task.WhenAll(tasks);
        }
    }
}