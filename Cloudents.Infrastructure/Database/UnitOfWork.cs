using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Exceptions;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database
{
    [UsedImplicitly]
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ITransaction _transaction;
        private readonly ISession _session;

        public UnitOfWork(ISession session)
        {
            _session = session;
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
                await _transaction.CommitAsync(token).ConfigureAwait(false);
            }
            catch (GenericADOException ex) when (ex.InnerException is SqlException sql && sql.Number == 2601)
            {
                //if (ex.InnerException is SqlException sql && sql.Number == 2601)
                //{
                throw new DuplicateRowException("Duplicate row", sql);
                //}
                //throw;
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
