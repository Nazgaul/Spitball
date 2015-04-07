using System;
using System.Globalization;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal class Invite2 : IMail2
    {
        private readonly IMailComponent m_MailComponent;
        private readonly ISendPush m_SendPush;
        public Invite2(IMailComponent mailComponent, ISendPush sendPush)
        {
            m_MailComponent = mailComponent;
            m_SendPush = sendPush;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data)
        {
            var parameters = data as InviteMailData;
            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }
            var userImage = parameters.InviterImage ?? "https://az32006.vo.msecnd.net/zboxprofilepic/DefaultEmailImage.jpg";

            if (parameters.ReceiverId.HasValue)
            {
                await  m_SendPush.SendInviteNotification(parameters.InviterName, parameters.BoxName,
                    parameters.ReceiverId.Value);
            }

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
