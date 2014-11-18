using System;
using System.Data;
using NHibernate;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.UnitsOfWork;

namespace Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork
{
    public class UnitOfWorkImplementor : IUnitOfWorkImplementor
    {
        private readonly IUnitOfWorkFactory m_Factory;
        private readonly ISession m_Session;

        public UnitOfWorkImplementor(IUnitOfWorkFactory factory, ISession session)
        {
            m_Factory = factory;
            m_Session = session;
        }
        public void Dispose()
        {
            m_Factory.DisposeUnitOfWork(this);
            m_Session.Dispose();
        }
        public void IncrementUsages()
        {
            throw new NotImplementedException();
        }
        public void Flush()
        {
            m_Session.Flush();
        }

        public bool IsInActiveTransaction
        {
            get { return m_Session.Transaction.IsActive; }
        }

        public IUnitOfWorkFactory Factory
        {
            get { return m_Factory; }
        }

        public ISession Session
        {
            get
            {
                return m_Session;
            }
        }

        public IGenericTransaction BeginTransaction()
        {
            return new GenericTransaction(m_Session.BeginTransaction());
        }

        public IGenericTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return new GenericTransaction(m_Session.BeginTransaction(isolationLevel));
        }

        public void TransactionalFlush()
        {
            TransactionalFlush(IsolationLevel.ReadCommitted);
        }

        public void TransactionalFlush(IsolationLevel isolationLevel)
        {
            // $$$$$$$$$$$$$$$$ gns: take this, when making thread safe! $$$$$$$$$$$$$$
            //IGenericTransaction tx = UnitOfWork.Current.BeginTransaction(isolationLevel);   

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
