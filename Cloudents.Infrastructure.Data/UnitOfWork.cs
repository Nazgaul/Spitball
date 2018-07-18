using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate;

namespace Cloudents.Infrastructure.Data
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
            if (!_transaction.IsActive)
            {
                throw new InvalidOperationException("No active transaction");
            }
            await _transaction.CommitAsync(token).ConfigureAwait(false);
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
