using System;
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
            _transaction = Session.BeginTransaction();
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

    public sealed class ReadonlyStatelessSession
    {
        public IStatelessSession Session { get; }

        private readonly ITransaction _transaction;

        public ReadonlyStatelessSession(IStatelessSession session)
        {
            Session = session;
            _transaction = Session.BeginTransaction();
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