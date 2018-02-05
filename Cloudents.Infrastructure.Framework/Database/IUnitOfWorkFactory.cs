using NHibernate;

namespace Cloudents.Infrastructure.Framework.Database
{
    public interface IUnitOfWorkFactory
    {
        ISession OpenSession();
    }

    public interface IUnitOfWork
    {
        ISession Session { get; }

        void FlagCommit();
    }
}