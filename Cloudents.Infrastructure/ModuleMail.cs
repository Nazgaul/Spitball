using Autofac;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Mail;

namespace Cloudents.Infrastructure
{
    public class ModuleMail : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<MailProvider>().As<IMailProvider>();
        }
    }
}