using Autofac;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Mail
{
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