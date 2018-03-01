using System;
using System.Data;
using JetBrains.Annotations;
using NHibernate;

namespace Cloudents.Infrastructure.Framework.Database
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ITransaction _transaction;
        private bool _isAlive = true;
        private bool _isCommitted;

        [UsedImplicitly]
        public delegate UnitOfWork Factory(Core.Enum.Database db);

        public UnitOfWork(Core.Enum.Database db, UnitOfWorkAutofacFactory factory)
        {
            var unitOfFactory = factory.GetInstance(db);

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
