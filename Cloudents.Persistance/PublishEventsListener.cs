using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate.Event;

namespace Cloudents.Persistence
{
    public class PublishEventsListener : IPostDeleteEventListener
        , IPostInsertEventListener, IPostUpdateEventListener, IPostCollectionUpdateEventListener

    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger _logger;

        public PublishEventsListener(IEventPublisher eventPublisher, ILogger logger)
        {
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task OnPostDeleteAsync(PostDeleteEvent @event, CancellationToken cancellationToken)
        {
            await PublishEvents(@event.Entity, cancellationToken);
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
            var t = PublishEvents(@event.Entity, CancellationToken.None);
            Task.WaitAll(t);
        }

        public async Task OnPostUpdateAsync(PostUpdateEvent @event, CancellationToken cancellationToken)
        {
            await PublishEvents(@event.Entity, cancellationToken);
        }

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            var t = PublishEvents(@event.Entity, CancellationToken.None);
            Task.WaitAll(t);
        }

        public async Task OnPostUpdateCollectionAsync(PostCollectionUpdateEvent @event, CancellationToken cancellationToken)
        {
            await PublishEvents(@event.AffectedOwnerOrNull, cancellationToken);
        }

        public void OnPostUpdateCollection(PostCollectionUpdateEvent @event)
        {
            var t =  PublishEvents(@event.AffectedOwnerOrNull, CancellationToken.None);
            Task.WaitAll(t);
        }


        private async Task PublishEvents(object entity, CancellationToken cancellationToken)
        {
            if (entity is IAggregateRoot p)
            {
                foreach (var ev in p.DomainEvents.Distinct())
                {
                    //Nhibernate doesn't support multiple async
                    try
                    {
                        await _eventPublisher.PublishAsync(ev, cancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.Exception(e);


                    }

                }
                p.ClearEvents();
            }
        }
    }
}