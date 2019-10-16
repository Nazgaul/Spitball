using Cloudents.Command;
using Cloudents.Command.StudyRooms;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Query;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
