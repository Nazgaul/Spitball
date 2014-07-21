using System;
using System.Globalization;
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

        public bool Excecute(BaseMailData data)
        {
            var parameters = data as InviteToCloudentsData;

            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }

            //obselete
            if (string.IsNullOrEmpty(parameters.SenderEmail))
            {
                m_MailComponent.GenerateAndSendEmail(parameters.EmailAddress,
                    new InvitationToCloudentsMailParams(parameters.SenderName, parameters.SenderImage,
                   new CultureInfo(parameters.Culture)));
            }
            else
            {
                m_MailComponent.GenerateAndSendEmail(parameters.EmailAddress,
                 new InvitationToCloudentsMailParams(parameters.SenderName, parameters.SenderImage,
                new CultureInfo(parameters.Culture), parameters.SenderEmail));
            }

            return true;
        }
    }
}
