using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    internal class Message2 : IMail2
    {
        private readonly IMailComponent m_MailComponent;
        private readonly ISendPush m_SendPush;
        public Message2(IMailComponent mailComponent, ISendPush sendPush)
        {
            m_MailComponent = mailComponent;
            m_SendPush = sendPush;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            var parameters = data as MessageMailData;

            if (parameters == null) return true;
            var userImage = parameters.SenderUserImage ?? "https://az32006.vo.msecnd.net/zboxprofilepic/DefaultEmailImage.jpg";
            var t1 = m_MailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                  new MessageMailParams(parameters.Message, parameters.SenderUserName,
                      new CultureInfo(parameters.Culture), parameters.SenderUserEmail, userImage), token);

            Task t2;// = Infrastructure.Extensions.TaskExtensions.CompletedTask;
            if (string.IsNullOrEmpty(parameters.Message))
            {
                t2 =
                    m_SendPush.SendChatFileNotificationAsync(parameters.SenderUserName, parameters.ConversationId,
                        new[] { parameters.UserId });
            }
            else
            {

                t2 = m_SendPush.SendChatMessageNotificationAsync(parameters.SenderUserName,
                    parameters.Message, parameters.ConversationId, new[] { parameters.UserId });
            }
            await Task.WhenAll(t1, t2);
            return true;

        }
    }
}
