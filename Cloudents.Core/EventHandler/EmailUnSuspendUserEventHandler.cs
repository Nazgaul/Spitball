using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class EmailUnSuspendUserEventHandler : EmailEventHandler, IEventHandler<UserUnSuspendEvent>
    {
        public EmailUnSuspendUserEventHandler(IQueueProvider serviceBusProvider) : base(serviceBusProvider)
        {
        }

        public async Task HandleAsync(UserUnSuspendEvent eventMessage, CancellationToken token)
        {
            await SendEmail(
                  new UnSuspendUserEmail(eventMessage.User.Email, eventMessage.User.Language), token);
        }
    }
}
