using System;
using System.Data;
using NHibernate;

namespace Cloudents.Infrastructure.Data.Query
{
    public sealed class ReadonlySession : IDisposable
    {
        public ISession Session { get; }

        private readonly ITransaction _transaction;

        public ReadonlySession(ISession session)
        {
            Session = session;
            _transaction = Session.BeginTransaction(IsolationLevel.ReadCommitted);
            Session.DefaultReadOnly = true;
            Session.FlushMode = FlushMode.Manual;
        }

        public void Dispose()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            Session?.Dispose();
        }
    }

    public sealed class ReadonlyStatelessSession : IDisposable
    {
        public IStatelessSession Session { get; }

        private readonly ITransaction _transaction;

        public ReadonlyStatelessSession(IStatelessSession session)
        {
            Session = session;
            _transaction = Session.BeginTransaction(IsolationLevel.ReadCommitted);
            //Session.DefaultReadOnly = true;
            //Session.FlushMode = FlushMode.Manual;
        }

        public void Dispose()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            Session?.Dispose();
        }
    }
}