using Autofac;
using Zbang.Zbox.Infrastructure.Data.Repositories;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Infrastructure.Data
{
    //public static class RegisterIoc
    //{
    //    public static void Register()
    //    {
    //        var ioc = IocFactory.IocWrapper;

    //        ioc.RegisterGeneric(typeof(IRepository<>), typeof(NHibernateRepository<>));
    //        ioc.RegisterGeneric(typeof(IDocumentDbRepository<>), typeof(DocumentDbRepository<>));
      

    //    }
    //}

    public class DataModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(NHibernateRepository<>)).As(typeof(IRepository<>));
            builder.RegisterGeneric(typeof(DocumentDbRepository<>)).As(typeof(IDocumentDbRepository<>));
            //builder.RegisterType<MediaSevicesProvider>().As<IMediaServicesProvider>().SingleInstance();
            //builder.RegisterType<BlobProvider>().As<IBlobProvider>().As<ICloudBlockProvider>().InstancePerLifetimeScope().AutoActivate();
            //builder.RegisterType<TableProvider>().As<ITableProvider>();
            //builder.RegisterType<QueueProvider>().As<IQueueProvider>().As<IQueueProviderExtract>();
            //builder.RegisterType<LocalStorageProvider>().As<ILocalStorageProvider>();
            //builder.RegisterType<Blob.IdGenerator>().As<IdGenerator.IIdGenerator>().SingleInstance();
            //builder.RegisterGeneric(typeof(BlobProvider2<>)).As(typeof(IBlobProvider2<>));
        }
    }
}
