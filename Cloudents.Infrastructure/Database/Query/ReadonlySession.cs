using System;
using System.Data;
using NHibernate;

namespace Cloudents.Infrastructure.Database.Query
{
    public sealed class ReadonlySession : IDisposable
    {
        public ISession Session { get; }

        private readonly ITransaction _transaction;

        public ReadonlySession(ISession session)
        {
            Session = session;
            _transaction = Session.BeginTransaction(IsolationLevel.ReadUncommitted);
            Session.DefaultReadOnly = true;
            Session.FlushMode = FlushMode.Manual;
        }

        public void Dispose()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
            Session?.Dispose();
        }
    }
}