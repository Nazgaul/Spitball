﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Exceptions;

namespace Cloudents.Persistance
{
    [UsedImplicitly]
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ITransaction _transaction;
        private readonly ISession _session;
        private readonly IEventPublisher _eventPublisher;
        private readonly IEventStore _eventStore;

        public UnitOfWork(ISession session, IEventPublisher eventPublisher, IEventStore eventStore)
        {
            _session = session;
            _eventPublisher = eventPublisher;
            _eventStore = eventStore;
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


            foreach (var @event in _eventStore)
            {
                await _eventPublisher.PublishAsync(@event, token);
            }
            //await PublishEventsAsync(token).ConfigureAwait(false);
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
