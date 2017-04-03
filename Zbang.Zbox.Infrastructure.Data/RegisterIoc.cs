using Autofac;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Infrastructure.Data
{
    public class DataModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(NHibernateRepository<>)).As(typeof(IRepository<>));
            builder.RegisterGeneric(typeof(DocumentDbRepository<>)).As(typeof(IDocumentDbRepository<>));

            builder.RegisterType<UnitOfWork>().AsSelf().As<IStartable>().SingleInstance().AutoActivate();
        }
    }
}
