using System;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal class Welcome : Imail2
    {
        private readonly IMailComponent m_MailComponent;
        public Welcome(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }
        public bool Excecute(BaseMailData data)
        {
            var parameters = data as WelcomeMailData;
            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }

            
            m_MailComponent.GenerateAndSendEmail(parameters.EmailAddress,new WelcomeMailParams(parameters.UserName,new  CultureInfo(parameters.Culture)));

            return true;

        }
    }
}
