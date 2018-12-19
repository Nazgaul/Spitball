using Autofac;
using Cloudents.Application.Attributes;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Storage;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Storage
{
    [ModuleRegistration(Application.Enum.System.Console)]
    [ModuleRegistration(Application.Enum.System.WorkerRole)]
    [ModuleRegistration(Application.Enum.System.Function)]
    [ModuleRegistration(Application.Enum.System.Web)]
    [ModuleRegistration(Application.Enum.System.Admin)]
    [ModuleRegistration(Application.Enum.System.IcoSite)]
    [UsedImplicitly]
    public class ModuleStorage : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var key = c.Resolve<IConfigurationKeys>().Storage;
                return new CloudStorageProvider(key);
            }).SingleInstance().AsImplementedInterfaces();

            builder.RegisterType<BlobProvider>().AsImplementedInterfaces();
            builder.RegisterType<QueueProvider>().AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(BlobProviderContainer<>)).AsImplementedInterfaces();
            builder.RegisterType<ServiceBusProvider>().As<IServiceBusProvider>();
        }
    }

    [ModuleRegistration(Application.Enum.System.WorkerRole)]
    public class ModuleTempStorage : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var key = c.Resolve<IConfigurationKeys>().LocalStorageData;
                return new TempStorageProvider(c.Resolve<ILogger>(), key);
            }).As<ITempStorageProvider>();
        }
    }
}