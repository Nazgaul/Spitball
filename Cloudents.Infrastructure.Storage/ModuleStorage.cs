using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Storage
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Api)]
    [ModuleRegistration(Core.Enum.System.Web)]
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

            builder.RegisterGeneric(typeof(BlobProvider<>)).AsImplementedInterfaces();

            //builder.Register(c =>
            //{
            //    var key = c.Resolve<IConfigurationKeys>().LocalStorageData;
            //    return new TempStorageProvider(c.Resolve<ILogger>(), key);
            //}).As<ITempStorageProvider>();
        }
    }
}