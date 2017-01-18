using System;
using System.Collections.Generic;
using NHibernate.Event.Default;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Infrastructure.Data.Events
{
    [Serializable]
    internal class ZboxDeleteEventListener : DefaultDeleteEventListener
    {
        protected override void DeleteEntity(NHibernate.Event.IEventSource session, object entity,
            NHibernate.Engine.EntityEntry entityEntry,
            bool isCascadeDeleteEnabled,
            NHibernate.Persister.Entity.IEntityPersister persister,
            ISet<object> transientEntities)
        {
            var dirty = entity as IDirty;
            if (dirty != null)
            {
                if (dirty.ShouldMakeDirty == null)
                {
                    dirty.IsDirty = Enums.DirtyState.Delete;
                }
                else if (dirty.ShouldMakeDirty())
                {
                    dirty.IsDirty = Enums.DirtyState.Delete;
                }
            }
            var deletable = entity as ISoftDelete;
            if (deletable != null)
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
