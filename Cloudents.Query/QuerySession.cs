using NHibernate;
using System;
using System.Data;

namespace Cloudents.Query
{

    public sealed class ReadDbTransaction : IDisposable
    {
        private readonly ITransaction _transaction;
        public ReadDbTransaction(IStatelessSession session)
        {
            _transaction = session.BeginTransaction(IsolationLevel.ReadUncommitted);
        }

        public void FinishTransaction()
        {
            if (_transaction == null) return;
            if (_transaction.IsActive)
            {
                _transaction.Rollback();
            }
        }

        
        public void Dispose()
        {
            if (_transaction?.IsActive == true)
            {
                //_transaction.Rollback();
                _transaction?.Dispose();
            }
        }
    }
}