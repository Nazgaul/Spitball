using System.Globalization;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal class Message2 : Imail2
    {
        private readonly IMailComponent m_MailComponent;
        public Message2(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }
        public bool Excecute(BaseMailData data)
        {
            var parameters = data as MessageMailData;
            //Throw.OnNull(parameters, "MessageMailData");
            if (parameters != null)
            {
                m_MailComponent.GenerateAndSendEmail(parameters.EmailAddress,
                    new MessageMailParams(parameters.Message, parameters.SenderUserName,
                   new CultureInfo(parameters.Culture)));
            }
            var parameters2 = data as MessageMailData2;
            if (parameters2 != null)
            {
                m_MailComponent.GenerateAndSendEmail(parameters2.EmailAddress,
                    new MessageMailParams(parameters2.Message, parameters2.SenderUserName,
                   new CultureInfo(parameters2.Culture), parameters2.SenderUserEmail, parameters2.SenderUserImage));
            }

            return true;

        }
    }
}
