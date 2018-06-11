﻿using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Framework
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Web)]
    //[ModuleRegistration(Core.Enum.System.Function)]
    public class ModuleServiceBus : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceBusProvider>().As<IServiceBusProvider>().As<IStartable>().SingleInstance().AutoActivate();
            base.Load(builder);
        }
    }
}