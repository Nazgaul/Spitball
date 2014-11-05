using System.Globalization;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal class ChangeEmail : IMail2
    {
        private readonly IMailComponent m_MailComponent;
        public ChangeEmail(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }
        public bool Execute(BaseMailData data)
        {
            var parameters = data as ChangeEmailData;
            if (parameters == null)
            {
                throw new System.NullReferenceException("parameters");
            }

            m_MailComponent.GenerateAndSendEmail(parameters.EmailAddress,
                new ChangeEmailMailParams(parameters.Code,
               new CultureInfo(parameters.Culture)));

            return true;

        }
    }
}
