using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;

namespace Cloudents.Infrastructure.Framework.Database
{
    [Serializable]
    internal class DeleteEventListener : DefaultDeleteEventListener
    {
        protected override Task DeleteEntityAsync(IEventSource session, object entity, EntityEntry entityEntry, bool isCascadeDeleteEnabled,
            IEntityPersister persister, ISet<object> transientEntities, CancellationToken cancellationToken)
        {
            DeleteEntity(session, entity, entityEntry, isCascadeDeleteEnabled, persister, transientEntities);
            return Task.CompletedTask;
        }

        protected override void DeleteEntity(IEventSource session, object entity,
            EntityEntry entityEntry,
            bool isCascadeDeleteEnabled,
            IEntityPersister persister,
            ISet<object> transientEntities)
        {
            if (entity is IDirty dirty)
            {
                dirty.IsDirty = true;
            }

            if (entity is ISoftDelete deletable)
            {
                deletable.DeleteAssociation();
                deletable.IsDeleted = true;

                CascadeBeforeDelete(session, persister, deletable, entityEntry, transientEntities);
                CascadeAfterDelete(session, persister, deletable, transientEntities);
            }
            else
            {
                base.DeleteEntity(session, entity, entityEntry, isCascadeDeleteEnabled,
                                  persister, transientEntities);
            }
        }
    }
}
