using Autofac;
using Cloudents.Application.Attributes;
using Cloudents.Application.Interfaces;

namespace Cloudents.Infrastructure.Mail
{
    [ModuleRegistration(Application.Enum.System.Console)]
    [ModuleRegistration(Application.Enum.System.Function)]
    [ModuleRegistration(Application.Enum.System.WorkerRole)]
    [ModuleRegistration(Application.Enum.System.Web)]
    [ModuleRegistration(Application.Enum.System.MailGun)]
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