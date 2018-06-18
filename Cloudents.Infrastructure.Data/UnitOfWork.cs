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
       

        public UnitOfWork(ISession unitOfFactory)
        {
            _session = unitOfFactory;//.OpenSession();
            _transaction = _session.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        //public ISession Session
        //{
        //    get;
        //}

        public void Dispose()
        {
            _transaction.Dispose();
            _session.Dispose();
            //if (!_isAlive)
            //    return;

            //_isAlive = false;

            //try
            //{
            //    if (_isCommitted)
            //    {
            //        if (Marshal.GetExceptionCode() == 0)
            //        {
            //            _transaction.Commit();
            //        }
            //        else
            //        {
            //            _transaction.Rollback();
            //        }
            //        _isCommitted = false;
            //    }
            //}
            //finally
            //{
            //    _transaction.Dispose();
            //    Session.Dispose();
            //}
        }

        //public void FlagCommit()
        //{
        //    if (!_isAlive)
        //        return;

        //    _isCommitted = true;
        //}
        public async Task CommitAsync(CancellationToken token)
        {
            if (!_transaction.IsActive)
            {
                throw new InvalidOperationException("No active transation");
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
