using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Event;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Message.Email;
using Cloudents.Application.Storage;

namespace Cloudents.Application.EventHandler
{
    class EmailUnSuspendUserEventHandler : EmailEventHandler, IEventHandler<UserUnSuspendEvent>
    {
        public EmailUnSuspendUserEventHandler(IQueueProvider serviceBusProvider) : base(serviceBusProvider)
        {
        }

        public async Task HandleAsync(UserUnSuspendEvent eventMessage, CancellationToken token)
        {
            await SendEmail(
                  new UnSuspendUserEmail(eventMessage.User.Email, eventMessage.User.Culture), eventMessage.User, token);
        }
    }
}
