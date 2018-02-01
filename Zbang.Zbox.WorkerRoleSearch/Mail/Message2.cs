using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    internal class Message2 : IMail2
    {
        private readonly IMailComponent _mailComponent;
        private readonly ISendPush _sendPush;
        private readonly ILogger _logger;
        public Message2(IMailComponent mailComponent, ISendPush sendPush, ILogger logger)
        {
            _mailComponent = mailComponent;
            _sendPush = sendPush;
            _logger = logger;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            if (!(data is MessageMailData parameters)) return true;

            //if (Guid.TryParse(parameters.EmailAddress, out _))
            //{
            //    await DoJaredPushAsync(parameters).ConfigureAwait(false);
            //    return true;
            //}

            var userImage = parameters.SenderUserImage ?? "https://az32006.vo.msecnd.net/zboxprofilepic/DefaultEmailImage.jpg";
            var t1 = _mailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                  new MessageMailParams(parameters.Message, parameters.SenderUserName,
                      new CultureInfo(parameters.Culture), parameters.SenderUserEmail, userImage), token);

            Task t2;// = Infrastructure.Extensions.TaskExtensions.CompletedTask;
            if (string.IsNullOrEmpty(parameters.Message))
            {
                t2 =
                    _sendPush.SendChatFileNotificationAsync(parameters.SenderUserName, parameters.ConversationId,
                        new[] { parameters.UserId });
            }
            else
            {
                t2 = _sendPush.SendChatMessageNotificationAsync(parameters.SenderUserName,
                    parameters.Message, parameters.ConversationId, new[] { parameters.UserId });
            }
            try
            {
                await Task.WhenAll(t1, t2).ConfigureAwait(false);
            }
            catch (AggregateException ae)
            {
                _logger.Exception(ae.Flatten());
                return t1.IsCompleted || t2.IsCompleted;
            }
            return true;
        }
       
    }
}
