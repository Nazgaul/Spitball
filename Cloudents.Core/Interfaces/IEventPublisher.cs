using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T eventMessage, CancellationToken token) where T : IEvent;
        Task PublishAsync(IEvent eventMessage, CancellationToken token);
    }

    //public interface IEventStore : IEnumerable<IEvent>
    //{
    //    void Add(IEvent @event);

    //    //IEnumerable<IEvents> Get();
    //}
}