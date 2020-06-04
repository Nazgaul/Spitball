using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate.Event;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence
{
    [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting" , Justification = "Piple line of nhibernate")]
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
            await PublishEventsAsync(@event.Entity, cancellationToken);
        }



        public void OnPostDelete(PostDeleteEvent @event)
        {
            var t = PublishEventsAsync(@event.Entity, CancellationToken.None);
            Task.WaitAll(t);
        }

        public async Task OnPostInsertAsync(PostInsertEvent @event, CancellationToken cancellationToken)
        {
            await PublishEventsAsync(@event.Entity, cancellationToken);

        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            var t = PublishEventsAsync(@event.Entity, CancellationToken.None);
            Task.WaitAll(t);
        }

        public async Task OnPostUpdateAsync(PostUpdateEvent @event, CancellationToken cancellationToken)
        {
            await PublishEventsAsync(@event.Entity, cancellationToken);
        }

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            var t = PublishEventsAsync(@event.Entity, CancellationToken.None);
            Task.WaitAll(t);
        }

        public async Task OnPostUpdateCollectionAsync(PostCollectionUpdateEvent @event, CancellationToken cancellationToken)
        {
            await PublishEventsAsync(@event.AffectedOwnerOrNull, cancellationToken);
        }

        public void OnPostUpdateCollection(PostCollectionUpdateEvent @event)
        {
            var t = PublishEventsAsync(@event.AffectedOwnerOrNull, CancellationToken.None);
            Task.WaitAll(t);
        }


        private async Task PublishEventsAsync(object entity, CancellationToken cancellationToken)
        {
            if (entity is IEntity p)
            {

                //I want to clear the domain event since AsQueryable reinject the publish event and then again... and stack-overflow exception 
                var list = p.DomainEvents.Distinct().Distinct().ToList();
                p.ClearEvents();
                foreach (var ev in list)
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