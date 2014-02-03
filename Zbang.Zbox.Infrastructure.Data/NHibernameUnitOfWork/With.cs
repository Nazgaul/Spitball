using System;
using System.Data;
using Zbang.Zbox.Infrastructure.UnitsOfWork;

namespace Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork
{
    public static class With
    {
        //Methods
        public static void Transaction(IsolationLevel level, Action transactional)
        {
            using (UnitOfWork.Start())
            {
                // If we are already in a transaction, don't start a new one
                if (UnitOfWork.Current.IsInActiveTransaction)
                {
                    transactional();
                }
                else
                {
                    IGenericTransaction tx = UnitOfWork.Current.BeginTransaction(level);
                    try
                    {
                        transactional();
                        tx.Commit();
                    }
                    catch
                    {
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

        public static void Transaction(Action transactional)
        {
            Transaction(IsolationLevel.ReadCommitted, transactional);
        }
    }
}
