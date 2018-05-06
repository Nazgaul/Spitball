using System;
using NHibernate;

namespace Cloudents.Infrastructure.Database
{
    public interface IUnitOfWorkFactory
    {
        ISession OpenSession();
    }

    public interface IUnitOfWork : IDisposable
    {
        ISession Session { get; }

        void FlagCommit();
    }
}