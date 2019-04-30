using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    public class SmsChatMessageEventHandler : IEventHandler<OfflineChatMessageEvent>
    {
        private readonly IServiceBusProvider _serviceBusProvider;

        public SmsChatMessageEventHandler(IServiceBusProvider serviceBusProvider)
        {
            _serviceBusProvider = serviceBusProvider;
        }

        public async Task HandleAsync(OfflineChatMessageEvent eventMessage, CancellationToken token)
        {
            var user = eventMessage.ChatUser;
            
            var message = new SmsMessage(user.User.PhoneNumber, "You got unread message in www.spitball.co", SmsMessage.MessageType.Sms);
            await _serviceBusProvider.InsertMessageAsync(message, token);
        }
    }
}