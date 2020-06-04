using Cloudents.Core.Entities;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence
{
    [Serializable]
    internal class SoftDeleteEventListener : DefaultDeleteEventListener
    {
        protected override void DeleteEntity(IEventSource session, object entity, EntityEntry entityEntry, bool isCascadeDeleteEnabled,
            IEntityPersister persister, ISet<object> transientEntities)
        {
            if (entity is ISoftDelete deletable)
            {
                DeleteItem(deletable);


                CascadeBeforeDelete(session, persister, deletable, entityEntry, transientEntities);
                CascadeAfterDelete(session, persister, deletable, transientEntities);
            }
            else
            {
                base.DeleteEntity(session, entity, entityEntry, isCascadeDeleteEnabled,
                    persister, transientEntities);
            }
        }

        protected override async Task DeleteEntityAsync(IEventSource session, object entity, EntityEntry entityEntry, bool isCascadeDeleteEnabled,
            IEntityPersister persister, ISet<object> transientEntities, CancellationToken cancellationToken)
        {
            if (entity is ISoftDelete deletable)
            {
                DeleteItem(deletable);


                await CascadeBeforeDeleteAsync(session, persister, deletable, entityEntry, transientEntities, cancellationToken);
                await CascadeAfterDeleteAsync(session, persister, deletable, transientEntities, cancellationToken);
            }
            else
            {
                await base.DeleteEntityAsync(session, entity, entityEntry, isCascadeDeleteEnabled,
                    persister, transientEntities, cancellationToken);
            }
        }

        private static void DeleteItem(ISoftDelete deletable)
        {
            //deletable.DeleteAssociation();
            //deletable.Item = deletable.Item;
            deletable.Delete();
        }
    }
}