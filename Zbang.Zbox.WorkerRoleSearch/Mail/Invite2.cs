using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
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

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            if (!(data is InviteMailData parameters))
            {
                throw new NullReferenceException("parameters");
            }
            var userImage = parameters.InviterImage ?? "https://az32006.vo.msecnd.net/zboxprofilepic/DefaultEmailImage.jpg";

            if (parameters.ReceiverId.HasValue)
            {
                await m_SendPush.SendInviteNotificationAsync(parameters.InviterName, parameters.BoxName, parameters.BoxId,
                    parameters.ReceiverId.Value).ConfigureAwait(false);
            }

            var inviteeEmail = parameters.InviterEmail;
            if (inviteeEmail.Contains("yahoo"))
            {
                inviteeEmail = MailParameters.DefaultEmail;
            }
            await m_MailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                new InviteMailParams(parameters.InviterName,
                    parameters.BoxName,
                    parameters.BoxUrl,
                    userImage,
                    new CultureInfo(parameters.Culture), inviteeEmail), token).ConfigureAwait(false);

            return true;
        }
    }
}
