﻿using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class ChangeConversationStatusCommandHandler : ICommandHandler<ChangeConversationStatusCommand>
    {
        private readonly IChatRoomRepository _repository;

        public ChangeConversationStatusCommandHandler(IChatRoomRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(ChangeConversationStatusCommand message, CancellationToken token)
        {
            var chatRoom = await _repository.GetChatRoomAsync(message.Identifier, token);
            chatRoom.Extra.Status = message.Status;
            await _repository.UpdateAsync(chatRoom, token);
        }
    }
}