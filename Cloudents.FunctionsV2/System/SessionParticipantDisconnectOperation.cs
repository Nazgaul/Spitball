using Cloudents.Command;
using Cloudents.Command.StudyRooms;
using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.System
{
    public class SessionParticipantDisconnectOperation : ISystemOperation<SessionDisconnectMessage>
    {
        private readonly ICommandBus _commandBus;

        public SessionParticipantDisconnectOperation(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public async Task DoOperationAsync(SessionDisconnectMessage msg, IBinder binder, CancellationToken token)
        {
            var command = new EndStudyRoomSessionAfterUserDisconnectedCommand(msg.Id);
            await _commandBus.DispatchAsync(command, token);
        }
    }
}
