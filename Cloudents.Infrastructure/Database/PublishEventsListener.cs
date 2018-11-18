﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using NHibernate.Event;

namespace Cloudents.Infrastructure.Database
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
            if (entity is IEvents p)
            {
                var tasks = new List<Task>();
                foreach (var ev in p.Events)
                {
                    tasks.Add( _eventPublisher.PublishAsync(ev, cancellationToken));
                    
                }
                await Task.WhenAll(tasks);
            }
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            var t =  PublishEvents(@event.Entity, CancellationToken.None);
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