using System;
using NHibernate;

namespace Cloudents.Infrastructure.Data
{
    public interface IUnitOfWork : IDisposable
    {
        ISession Session { get; }

        void FlagCommit();
    }

    public interface IUnitOfWorkFactory
    {
        ISession OpenSession();
    }
}