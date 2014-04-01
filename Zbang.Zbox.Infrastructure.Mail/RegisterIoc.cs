using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.Unity;

            ioc.RegisterType<IMailComponent, MailManager2>();
            ioc.RegisterType<IMailBuilder, WelcomeMail>(MailParameters.WelcomeResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, InviteMail>(MailParameters.InviteResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, ForgotPasswordMail>(MailParameters.ForgotPswResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, MessageMail>(MailParameters.MessageResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, UpdatesMail>(MailParameters.UpdateResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, ChangeEmailMail>(MailParameters.ChangeEmailResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, FlagItemMail>(MailParameters.FlagBadItemResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, InvitationToCloudentsMail>(MailParameters.InvitationToCloudentsResolver, LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IMailBuilder, PartnersMail>(MailParameters.PartnersResolver, LifeTimeManager.PerHttpRequest);

        }
    }
}
