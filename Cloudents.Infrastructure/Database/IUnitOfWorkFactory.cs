using NHibernate;

namespace Cloudents.Infrastructure.Database
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