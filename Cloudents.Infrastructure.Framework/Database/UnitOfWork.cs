using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace Cloudents.Infrastructure.Framework.Database
{
    public class UnitOfWork :  IUnitOfWork, IDisposable
    {
        private readonly ITransaction _transaction;
        private bool _isAlive = true;
        private bool _isCommitted;

        public UnitOfWork(ISession session)
        {
            Session = session;
            _transaction = Session.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public ISession Session
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (!_isAlive)
                return;

            _isAlive = false;

            try
            {
                if (_isCommitted)
                {
                    _transaction.Commit();
                }
            }
            finally
            {
                _transaction.Dispose();
                Session.Dispose();
            }
        }

        public void FlagCommit()
        {
            if (!_isAlive)
                return;

            _isCommitted = true;
        }
    }
}
