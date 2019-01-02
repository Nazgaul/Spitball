using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate.Event;

namespace Cloudents.Persistance
{
    public class PublishEventsListener : IPostDeleteEventListener
        , IPostInsertEventListener, IPostUpdateEventListener

    {
        private readonly IEventPublisher _eventPublisher;

        public PublishEventsListener(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task OnPostDeleteAsync(PostDeleteEvent @event, CancellationToken cancellationToken)
        {
            await PublishEvents(@event.Entity, cancellationToken);
        }

        private async Task PublishEvents(object entity, CancellationToken cancellationToken)
        {
            if (entity is AggregateRoot p)
            {
                foreach (var ev in p.Events)
                {
                    //Nhibernate doesn't support multiple async
                    await _eventPublisher.PublishAsync(ev, cancellationToken);

                }
            }
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            var t = PublishEvents(@event.Entity, CancellationToken.None);
            Task.WaitAll(t);
        }

        public async Task OnPostInsertAsync(PostInsertEvent @event, CancellationToken cancellationToken)
        {
            await PublishEvents(@event.Entity, cancellationToken);

        }

        public void OnPostInsert(PostInsertEvent @event)
        {
        }

        public async Task OnPostUpdateAsync(PostUpdateEvent @event, CancellationToken cancellationToken)
        {
            await PublishEvents(@event.Entity, cancellationToken);
        }

        public void OnPostUpdate(PostUpdateEvent @event)
        {
        }
    }
}