using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class ChangeConversationAssignCommandHandler : ICommandHandler<ChangeConversationAssignCommand>
    {
        private readonly IChatRoomRepository _repository;

        public ChangeConversationAssignCommandHandler(IChatRoomRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(ChangeConversationAssignCommand message, CancellationToken token)
        {
            var chatRoom = await _repository.GetChatRoomAsync(message.Identifier, token);
            chatRoom.Extra.AssignTo = message.AssignTo;
            await _repository.UpdateAsync(chatRoom, token);
        }
    }
}
