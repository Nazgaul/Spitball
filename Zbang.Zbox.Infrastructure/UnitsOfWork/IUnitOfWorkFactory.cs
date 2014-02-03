
using NHibernate.Cfg;
using NHibernate;

namespace Zbang.Zbox.Infrastructure.UnitsOfWork
{
    public interface IUnitOfWorkFactory
    {
        Configuration Configuration { get; }
        ISessionFactory SessionFactory { get; }
        ISession CurrentSession { get; set; }

        IUnitOfWork Create();
        void DisposeUnitOfWork(IUnitOfWorkImplementor adapter);
    }
}
