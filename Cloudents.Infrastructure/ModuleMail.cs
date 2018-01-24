using Autofac;
using Cloudents.Infrastructure.Mail;

namespace Cloudents.Infrastructure
{
    public class ModuleMail : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<MailProvider>().AsImplementedInterfaces();
        }
    }
}