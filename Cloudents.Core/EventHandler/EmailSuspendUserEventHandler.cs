using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Event;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Message.Email;
using Cloudents.Application.Storage;

namespace Cloudents.Application.EventHandler
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class EmailSuspendUserEventHandler : EmailEventHandler, IEventHandler<UserSuspendEvent>
    {
        


        public EmailSuspendUserEventHandler(IQueueProvider serviceBusProvider): base(serviceBusProvider)
        {
        }

        public async Task HandleAsync(UserSuspendEvent eventMessage, CancellationToken token)
        {
            await SendEmail(
                  new SuspendUserEmail(eventMessage.User.Email, eventMessage.User.Culture), eventMessage.User, token);
        }
    }
}
