using System;

namespace Zbang.Zbox.Infrastructure.UnitsOfWork
{
    public interface IGenericTransaction: IDisposable
    {
        void Commit();
        void Rollback();
    }
}
