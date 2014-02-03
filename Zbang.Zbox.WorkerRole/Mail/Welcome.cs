using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Exceptions;
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
        public bool Excecute(Infrastructure.Transport.BaseMailData data)
        {
            var parameters = data as WelcomeMailData;
            Throw.OnNull(parameters, "WelcomeMailparmas");

            
            m_MailComponent.GenerateAndSendEmail(parameters.EmailAddress,new WelcomeMailParams(parameters.UserName,new  CultureInfo(parameters.Culture)));

            return true;

        }
    }
}
