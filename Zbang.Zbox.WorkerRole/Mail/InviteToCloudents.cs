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
    internal class InviteToCloudents : Imail2
    {
        private readonly IMailComponent m_MailComponent;
        public InviteToCloudents(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }

        public bool Excecute(Infrastructure.Transport.BaseMailData data)
        {
            var parameters = data as InviteToCloudentsData;
            Throw.OnNull(parameters, "InviteToCloudents");

            m_MailComponent.GenerateAndSendEmail(parameters.EmailAddress,
                new InvitationToCloudentsMailParams(parameters.SenderName, parameters.SenderImage,
               new CultureInfo(parameters.Culture)));

            return true;
        }
    }
}
