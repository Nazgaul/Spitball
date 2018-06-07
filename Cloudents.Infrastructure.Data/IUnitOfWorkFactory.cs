using NHibernate;

namespace Cloudents.Infrastructure.Data
{
    public interface IUnitOfWorkFactory
    {
        ISession OpenSession();
    }
}