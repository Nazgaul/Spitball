using System;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Data.Events
{
    [Serializable]
    internal class AuditEventListener : IPreUpdateEventListener, IPreInsertEventListener
    {
        public bool OnPreInsert(PreInsertEvent @event)
        {
            var dirty = @event.Entity as IDirty;
            if (dirty == null) return false;
            if (dirty.ShouldMakeDirty == null)
            {
                MakeDirty(@event, @event.State, dirty, DirtyState.New);
                return false;
            }
            if (dirty.ShouldMakeDirty())
            {
                MakeDirty(@event, @event.State, dirty, DirtyState.New);
            }
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var dirty = @event.Entity as IDirty;
            if (dirty == null) return false;
            if (dirty.ShouldMakeDirty == null)
            {
                MakeDirty(@event, @event.State, dirty, DirtyState.Update);
                return false;
            }
            if (dirty.ShouldMakeDirty())
            {
                MakeDirty(@event, @event.State, dirty,DirtyState.Update);
            }


            return false;
        }

        private void MakeDirty(IPreDatabaseOperationEventArgs @event, object[] state, IDirty dirty, DirtyState dirtyState)
        {
            if (dirty.IsDirty == DirtyState.New)
            {
                return;
            }
            dirty.IsDirty = dirtyState;
            TraceLog.WriteInfo($"making dirty {Environment.StackTrace}");
            Set(@event.Persister, state, "IsDirty", dirtyState);
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