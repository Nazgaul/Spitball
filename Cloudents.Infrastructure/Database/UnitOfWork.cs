using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Exceptions;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database
{
    [UsedImplicitly]
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ITransaction _transaction;
        private readonly ISession _session;
        private readonly IEventPublisher _eventPublisher;

        public UnitOfWork(ISession session, IEventPublisher eventPublisher)
        {
            _session = session;
            _eventPublisher = eventPublisher;
            _transaction = _session.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void Dispose()
        {
            _transaction.Dispose();
            _session.Dispose();
        }

        public async Task CommitAsync(CancellationToken token)
        {

            try
            {
                if (!_transaction.IsActive)
                {
                    throw new InvalidOperationException("No active transaction");
                }

                await _transaction.CommitAsync(token);
            }
            /*SELECT * FROM sys.messages
WHERE text like '%duplicate%' and text like '%key%' and language_id = 1033*/
            catch (GenericADOException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
            {
                throw new DuplicateRowException("Duplicate row", sql);
            }

            //await PublishEventsAsync(token).ConfigureAwait(false);
        }

        public async Task PublishEventsAsync(CancellationToken token)
        {
            //TODO :Taken https://gist.github.com/oguzhaneren/202267362af027a6e523 temp solution
            //We should use Domain event taken from https://enterprisecraftsmanship.com/2018/06/13/ef-core-vs-nhibernate-ddd-perspective/
            //But currently no async event works.
            //https://github.com/nhibernate/nhibernate-core/issues/1826
            var sessionImpl = _session.GetSessionImplementation();
            var persistenceContext = sessionImpl.PersistenceContext;
            var changedObjects = (from EntityEntry entityEntry in persistenceContext.EntityEntries.Values
                                  select persistenceContext.GetEntity(entityEntry.EntityKey))
                .OfType<IEvents>()
                .Where(ev => ev.Events.Count > 0)
                .ToList();

            foreach (var entity in changedObjects)
            {
                foreach (var ev in entity.Events)
                {
                    await _eventPublisher.PublishAsync(ev, token).ConfigureAwait(false);
                    // DomainEvents.Publish(ev);
                }
                entity.Events.Clear();
            }
        }

        public async Task RollbackAsync(CancellationToken token)
        {
            if (_transaction.IsActive)
            {
                await _transaction.RollbackAsync(token).ConfigureAwait(false);
            }
        }
    }
}
