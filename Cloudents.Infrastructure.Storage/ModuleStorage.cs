﻿using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Storage
{
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

    //[ModuleRegistration(Application.Enum.System.WorkerRole)]
    //public class ModuleTempStorage : Module
    //{
    //    protected override void Load(ContainerBuilder builder)
    //    {
    //        builder.Register(c =>
    //        {
    //            var key = c.Resolve<IConfigurationKeys>().LocalStorageData;
    //            return new TempStorageProvider(c.Resolve<ILogger>(), key);
    //        }).As<ITempStorageProvider>();
    //    }
    //}
}