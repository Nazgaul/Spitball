using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class SessionParticipantDisconnectEventHandler : IEventHandler<SessionParticipantDisconnectEvent>
    {
        private readonly IQueueProvider _serviceBusProvider;
        private const double TimeToDisconnectInMinutes = 5;
        public SessionParticipantDisconnectEventHandler(IQueueProvider serviceBusProvider)
        {
            _serviceBusProvider = serviceBusProvider;
        }

        public async Task HandleAsync(SessionParticipantDisconnectEvent disconnectMessage, CancellationToken token)
        {
            var message = new SessionDisconnectMessage(disconnectMessage.SessionDisconnect.Id);
            await _serviceBusProvider.InsertMessageAsync(message, TimeSpan.FromMinutes(TimeToDisconnectInMinutes), token);
        }
    }
}
