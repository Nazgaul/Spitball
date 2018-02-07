using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Extension;
using NHibernate.Event;
using NHibernate.Persister.Entity;

namespace Cloudents.Infrastructure.Framework.Database
{
    [Serializable]
    internal class InsertUpdateEventListener : IPreUpdateEventListener, IPreInsertEventListener
    {
        public Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken cancellationToken)
        {
            var retVal = TaskCompleted.CompletedTaskFalse;
            if (!(@event.Entity is IDirty dirty)) return retVal;
            MakeDirty(@event, @event.State, dirty);
            return retVal;
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            if (!(@event.Entity is IDirty dirty)) return false;
            //if (dirty.ShouldMakeDirty == null)
            //{
            //    MakeDirty(@event, @event.State, dirty);
            //    return false;
            //}
            //if (dirty.ShouldMakeDirty())
            //{
                MakeDirty(@event, @event.State, dirty);
            //}
            return false;
        }

        public Task<bool> OnPreUpdateAsync(PreUpdateEvent @event, CancellationToken cancellationToken)
        {
            var retVal = TaskCompleted.CompletedTaskFalse;
            var dirty = @event.Entity as IDirty;
            if (dirty?.ShouldMakeDirty == null)
            {
                //MakeDirty(@event, @event.State, dirty);
                return retVal;
            }
            if (dirty.ShouldMakeDirty())
            {
                MakeDirty(@event, @event.State, dirty);
            }
            return retVal;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var dirty = @event.Entity as IDirty;
            if (dirty?.ShouldMakeDirty == null)
            {
                //MakeDirty(@event, @event.State, dirty);
                return false;
            }
            if (dirty.ShouldMakeDirty())
            {
                MakeDirty(@event, @event.State, dirty);
            }
            return false;
        }

        private void MakeDirty(IPreDatabaseOperationEventArgs @event, object[] state, IDirty dirty)
        {
            dirty.IsDirty = true;
            //TraceLog.WriteInfo($"making dirty count {state.Length} trace: {Environment.StackTrace}");
            Set(@event.Persister, state, "IsDirty", true);
        }

        private static void Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }
    }
}