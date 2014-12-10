using System;
using System.Collections.Generic;
using NHibernate.Event.Default;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Infrastructure.Data
{
    [Serializable]
    class ZboxDeleteEventListener : DefaultDeleteEventListener
    {
        protected override void DeleteEntity(NHibernate.Event.IEventSource session, object entity,
            NHibernate.Engine.EntityEntry entityEntry,
            bool isCascadeDeleteEnabled,
            NHibernate.Persister.Entity.IEntityPersister persister,
            ISet<object> transientEntities)
        {
            var deletable = entity as ISoftDeletable;
            if (deletable != null)
            {
                var e = deletable;
                e.IsDeleted = true;

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
