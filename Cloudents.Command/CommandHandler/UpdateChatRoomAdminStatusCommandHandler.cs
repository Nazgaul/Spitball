using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class UpdateChatRoomAdminStatusCommandHandler : ICommandHandler<UpdateChatRoomAdminStatusCommand>
    {
        private readonly IChatRoomRepository _repository;
        public UpdateChatRoomAdminStatusCommandHandler(IChatRoomRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(UpdateChatRoomAdminStatusCommand message, CancellationToken token)
        {
            await _repository.UpdateNonDayOldConversationToActiveAsync(token);
        }
    }
}
