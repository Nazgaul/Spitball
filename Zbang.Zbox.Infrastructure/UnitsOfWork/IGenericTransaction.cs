using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.UnitsOfWork
{
    public interface IGenericTransaction: IDisposable
    {
        void Commit();
        void Rollback();
    }
}
