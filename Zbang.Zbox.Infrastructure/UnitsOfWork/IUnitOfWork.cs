using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Zbang.Zbox.Infrastructure.UnitsOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void Flush();
        bool IsInActiveTransaction { get; }

        IGenericTransaction BeginTransaction();
        IGenericTransaction BeginTransaction(IsolationLevel isolationLevel);
        void TransactionalFlush();
        void TransactionalFlush(IsolationLevel isolationLevel);
    }
}
