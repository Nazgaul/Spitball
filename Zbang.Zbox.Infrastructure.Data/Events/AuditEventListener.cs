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
                dirty.IsDirty = true;
                Set(@event.Persister, @event.State, "IsDirty", true);
            } 
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var dirty = @event.Entity as IDirty;
            if (dirty != null)
            {
                dirty.IsDirty = true;
                Set(@event.Persister, @event.State, "IsDirty", true);
            }

          

            return false;
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