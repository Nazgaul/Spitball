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

        
        public void Dispose()
        {
            if (_transaction != null)
            {
                if (_transaction.IsActive)
                {
                    _transaction.Rollback();
                    _transaction?.Dispose();
                }
            }

            //Session?.Dispose();
        }
    }

    //public sealed class QuerySession : IDisposable
    //{
    //    private readonly Lazy<IStatelessSession> _statelessSession;



    //    public IStatelessSession StatelessSession
    //    {
    //        get
    //        {
    //            if (_statelessSession.IsValueCreated)
    //            {
    //                return _statelessSession.Value;
    //            }
    //            _transaction = _statelessSession.Value.BeginTransaction(IsolationLevel.ReadUncommitted);
    //            return _statelessSession.Value;
    //        }
    //    }

    //    private ITransaction? _transaction;



    //    public QuerySession(Lazy<IStatelessSession> statelessSession)
    //    {
    //        _statelessSession = statelessSession;


    //    }
    //    public void Dispose()
    //    {
    //        if (_transaction != null)
    //        {
    //            if (_transaction.IsActive)
    //            {
    //                _transaction.Rollback();
    //                _transaction?.Dispose();
    //            }
    //        }

    //        //Session?.Dispose();
    //    }
    //}
}