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
    internal class Invite2 : Imail2
    {
        private readonly IMailComponent m_MailComponent;
        public Invite2(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }
        public bool Excecute(Infrastructure.Transport.BaseMailData data)
        {
            var parameters = data as InviteMailData;
            Throw.OnNull(parameters, "InviteMailData");

            m_MailComponent.GenerateAndSendEmail(parameters.EmailAddress,
                new InviteMailParams(parameters.InvitorName,
                parameters.BoxName,
                parameters.BoxUrl,
               new CultureInfo(parameters.Culture)));

            return true;

        }
    }
}
