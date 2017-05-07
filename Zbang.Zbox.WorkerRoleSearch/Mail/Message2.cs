using System;
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
        private readonly IJaredPushNotification m_JaredPush;
        public Message2(IMailComponent mailComponent, ISendPush sendPush, IJaredPushNotification jaredPush)
        {
            m_MailComponent = mailComponent;
            m_SendPush = sendPush;
            m_JaredPush = jaredPush;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            var parameters = data as MessageMailData;

            if (parameters == null) return true;

            Guid temp;
            if (Guid.TryParse(parameters.EmailAddress, out temp))
            {
                //This is Jared user
                await DoJaredPushAsync(parameters).ConfigureAwait(false);
                return true;
            }

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
            await Task.WhenAll(t1, t2).ConfigureAwait(false);
            return true;

        }


        private Task DoJaredPushAsync(MessageMailData parameters)
        {
            Task t2;
            if (string.IsNullOrEmpty(parameters.Message))
            {
                t2 = m_JaredPush.SendChatMessagePushAsync(parameters.SenderUserName, parameters.Message,
                    parameters.ConversationId, parameters.UserId);
            }
            else
            {
                t2 = m_JaredPush.SendChatFilePushAsync(parameters.SenderUserName, parameters.ConversationId,
                    parameters.UserId);
            }
            return t2;
        }
    }
}
