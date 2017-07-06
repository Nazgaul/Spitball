using System;
using System.Data;
using NHibernate;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.UnitsOfWork;

namespace Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork
{
    public class UnitOfWorkImplementor : IUnitOfWorkImplementor
    {
        public UnitOfWorkImplementor(IUnitOfWorkFactory factory, ISession session)
        {
            Factory = factory;
            Session = session;
        }
        public void Dispose()
        {
            Factory.DisposeUnitOfWork(this);
            Session.Dispose();
        }
        public void IncrementUsages()
        {
            throw new NotImplementedException();
        }
        public void Flush()
        {
            Session.Flush();
        }

        public bool IsInActiveTransaction => Session.Transaction.IsActive;

        public IUnitOfWorkFactory Factory { get; }

        public ISession Session { get; }

        public IGenericTransaction BeginTransaction()
        {
            return new GenericTransaction(Session.BeginTransaction());
        }

        public IGenericTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return new GenericTransaction(Session.BeginTransaction(isolationLevel));
        }

        public void TransactionalFlush()
        {
            TransactionalFlush(IsolationLevel.ReadCommitted);
        }

        public void TransactionalFlush(IsolationLevel isolationLevel)
        {
            var tx = BeginTransaction(isolationLevel);

            try
            {
                tx.Commit();
            }
            catch(Exception ex)
            {
                TraceLog.WriteError(ex);
                tx.Rollback();
                throw;
            }
            finally
            {
                tx.Dispose();
            }
        }
    }
}
