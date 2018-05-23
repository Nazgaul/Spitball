using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Storage
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.WorkerRole)]
    [ModuleRegistration(Core.Enum.System.Web)]
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

            builder.Register(c =>
            {
                var key = c.Resolve<IConfigurationKeys>().LocalStorageData;
                return new TempStorageProvider(c.Resolve<ILogger>(), key);
            }).As<ITempStorageProvider>();
        }
    }
}