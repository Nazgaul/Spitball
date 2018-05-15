using System;
using NHibernate;

namespace Cloudents.Infrastructure.Database
{

    public interface IUnitOfWork : IDisposable
    {
        ISession Session { get; }

        void FlagCommit();
    }
}