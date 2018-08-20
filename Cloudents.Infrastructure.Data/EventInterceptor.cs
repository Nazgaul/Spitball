using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Event;

namespace Cloudents.Infrastructure.Data
{
    public class EventInterceptor : IPostInsertEventListener, IPostDeleteEventListener
    {
        private readonly IEventPublisher _eventPublisher;

        public EventInterceptor(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task OnPostInsertAsync(PostInsertEvent @event, CancellationToken cancellationToken)
        {
            //if (!(@event.Entity is IHaveEvent entity)) return;

            //foreach (var eventMessage in entity.Events)
            //{
            //   await _eventPublisher.PublishAsync(eventMessage, cancellationToken);
            //}
           
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            //throw new System.NotImplementedException();
        }

        public Task OnPostDeleteAsync(PostDeleteEvent @event, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            throw new System.NotImplementedException();
        }
    }
}