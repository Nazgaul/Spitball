using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class EmailSuspendUserEventHandler : EmailEventHandler, IEventHandler<UserSuspendEvent>
    {



        public EmailSuspendUserEventHandler(IQueueProvider serviceBusProvider) : base(serviceBusProvider)
        {
        }

        public async Task HandleAsync(UserSuspendEvent eventMessage, CancellationToken token)
        {
            await SendEmail(
                  new SuspendUserEmail(eventMessage.User.Email, eventMessage.User.Language), token);
        }
    }
}
