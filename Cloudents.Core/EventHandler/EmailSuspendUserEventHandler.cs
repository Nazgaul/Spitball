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
    public class EmailSuspendUserEventHandler : IEventHandler<UserSuspendEvent>
    {
        private readonly IQueueProvider _serviceBusProvider;


        public EmailSuspendUserEventHandler(IQueueProvider serviceBusProvider)
        {
            _serviceBusProvider = serviceBusProvider;
        }

        public async Task HandleAsync(UserSuspendEvent eventMessage, CancellationToken token)
        {
            await _serviceBusProvider.InsertMessageAsync(
                  new SuspendUserEmail(eventMessage.User.Email, eventMessage.User.Culture), token);
        }
    }
}
