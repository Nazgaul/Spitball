using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

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
            if (chatRoom.Extra == null)
            {
                chatRoom.Extra = new ChatRoomAdmin(chatRoom);
            }
            chatRoom.Extra.Status = message.Status;
           

            await _repository.UpdateAsync(chatRoom, token);
        }
    }
}