using System;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Infrastructure.Data.Events
{
    [Serializable]
    class AuditEventListener : IPreUpdateEventListener, IPreInsertEventListener
    {
        public bool OnPreInsert(PreInsertEvent @event)
        {
            var dirty = @event.Entity as IDirty;
            if (dirty != null)
            {
                if (dirty.ShouldMakeDirty == null)
                {
                    MakeDirty(@event, @event.State, dirty);
                    return false;
                }
                if (dirty.ShouldMakeDirty())
                {
                    MakeDirty(@event, @event.State, dirty);
                }
            }
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var dirty = @event.Entity as IDirty;
            if (dirty != null)
            {
                if (dirty.ShouldMakeDirty == null)
                {
                    MakeDirty(@event, @event.State, dirty);
                    return false;
                }
                if (dirty.ShouldMakeDirty())
                {
                    MakeDirty(@event, @event.State, dirty);
                }
            }



            return false;
        }

        private void MakeDirty(IPreDatabaseOperationEventArgs @event, object[] state, IDirty dirty)
        {
            dirty.IsDirty = true;
            Set(@event.Persister, state, "IsDirty", true);
        }

        private void Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }
    }
}