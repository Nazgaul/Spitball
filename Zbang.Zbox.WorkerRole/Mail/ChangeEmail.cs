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
    internal class ChangeEmail : Imail2
    {
        private readonly IMailComponent m_MailComponent;
        public ChangeEmail(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }
        public bool Excecute(Infrastructure.Transport.BaseMailData data)
        {
            var parameters = data as ChangeEmailData;
            Throw.OnNull(parameters, "ChangeEmailData");

            m_MailComponent.GenerateAndSendEmail(parameters.EmailAddress,
                new ChangeEmailMailParams(parameters.Code,
               new CultureInfo(parameters.Culture)));

            return true;

        }
    }
}
