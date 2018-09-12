﻿using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Mail
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Function)]
    [ModuleRegistration(Core.Enum.System.WorkerRole)]
    [ModuleRegistration(Core.Enum.System.Web)]
    [ModuleRegistration(Core.Enum.System.MailGun)]
    public class ModuleMail : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<MailProvider>().As<IMailProvider>();
            builder.RegisterType<SmsProvider>().As<ISmsProvider>().SingleInstance();
        }
    }
}