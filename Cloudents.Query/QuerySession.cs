using System;
using System.Data;
using NHibernate;

namespace Cloudents.Query
{
    public sealed class QuerySession : IDisposable
    {
        private readonly Lazy<ISession> _session;
        private readonly Lazy<IStatelessSession> _statelessSession;

        public ISession Session
        {
            get
            {
                if (_session.IsValueCreated)
                {
                    return _session.Value;
                }
                _transaction = _session.Value.BeginTransaction(IsolationLevel.ReadUncommitted);
                _session.Value.DefaultReadOnly = true;
                _session.Value.FlushMode = FlushMode.Manual;
                return _session.Value;
            }
        }

        public IStatelessSession StatelessSession
        {
            get
            {
                if (_statelessSession.IsValueCreated)
                {
                    return _statelessSession.Value;
                }
                _transaction = _statelessSession.Value.BeginTransaction(IsolationLevel.ReadUncommitted);
                return _statelessSession.Value;
            }
        }

        private ITransaction _transaction;

        

        public QuerySession(Lazy<ISession> session, Lazy<IStatelessSession> statelessSession)
        {
            _session = session;
            _statelessSession = statelessSession;


        }

      

        public void Dispose()
        {
            if (_transaction.IsActive)
            {
                _transaction?.Rollback();
                _transaction?.Dispose();
            }

            //Session?.Dispose();
        }
    }
}