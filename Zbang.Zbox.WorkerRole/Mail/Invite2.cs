using System;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal class Invite2 : IMail2
    {
        private readonly IMailComponent m_MailComponent;
        public Invite2(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }
        public bool Execute(BaseMailData data)
        {
            var parameters = data as InviteMailData;
            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }
            var userImage = parameters.InviterImage ?? "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/userpic9.jpg";

            if (string.IsNullOrEmpty(parameters.InviterEmail))
            {
                //obselete
                m_MailComponent.GenerateAndSendEmail(parameters.EmailAddress,
                    new InviteMailParams(parameters.InviterName,
                    parameters.BoxName,
                    parameters.BoxUrl,
                    userImage,
                   new CultureInfo(parameters.Culture)));
            }
            else
            {
                m_MailComponent.GenerateAndSendEmail(parameters.EmailAddress,
                    new InviteMailParams(parameters.InviterName,
                    parameters.BoxName,
                    parameters.BoxUrl,
                    userImage,
                   new CultureInfo(parameters.Culture), parameters.InviterEmail));
            }

            return true;

        }
    }
}
