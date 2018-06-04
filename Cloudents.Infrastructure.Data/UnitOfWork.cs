using System.Data;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using NHibernate;

namespace Cloudents.Infrastructure.Data
{
    [UsedImplicitly]
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ITransaction _transaction;
        private bool _isAlive = true;
        private bool _isCommitted;

       // [UsedImplicitly]
        //public delegate UnitOfWork Factory(Core.Enum.Database db);

        public UnitOfWork(IUnitOfWorkFactory unitOfFactory)
        {
            //var unitOfFactory = factory.GetInstance(db);

            Session = unitOfFactory.OpenSession();
            _transaction = Session.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public ISession Session
        {
            get;
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
                    if (Marshal.GetExceptionCode() == 0)
                    {
                        _transaction.Commit();
                    }
                    else
                    {
                        _transaction.Rollback();
                    }
                    _isCommitted = false;
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
