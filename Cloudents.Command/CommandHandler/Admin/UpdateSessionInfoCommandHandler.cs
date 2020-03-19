using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class UpdateSessionInfoCommandHandler : ICommandHandler<UpdateSessionInfoCommand>
    {
        private readonly IRepository<StudyRoomSession> _repository;
        public UpdateSessionInfoCommandHandler(IRepository<StudyRoomSession> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(UpdateSessionInfoCommand command, CancellationToken token)
        {
            var session = await _repository.GetAsync(command.SessionId, token);
            session.EditDuration(command.Minutes);

        }
    }
}
