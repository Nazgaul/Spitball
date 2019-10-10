using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

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
            if (chatRoom.Extra == null)
            {
                chatRoom.Extra = new ChatRoomAdmin(chatRoom);
            }

            if (string.IsNullOrEmpty(message.AssignTo))
            {
                chatRoom.Extra.AssignTo = null;
            }
            else
            {
                chatRoom.Extra.AssignTo = message.AssignTo;
            }

            await _repository.UpdateAsync(chatRoom, token);
        }
    }
}
