using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class CreateNoteCommandHandler : ICommandHandler<CreateNoteCommand>
    {
        private readonly IRepository<AdminNote> _noteRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IAdminUserRepository _adminUserRepository;
        public CreateNoteCommandHandler(IRepository<AdminNote> noteRepository, IRepository<User> userRepository,
            IAdminUserRepository adminUserRepository)
        {
            _noteRepository = noteRepository;
            _userRepository = userRepository;
            _adminUserRepository = adminUserRepository;
        }

        public async Task ExecuteAsync(CreateNoteCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var adminUser = await _adminUserRepository.GetAdminUserByEmailAsync(message.AdminEmail, token);
            var note = new AdminNote(message.Text, user, adminUser);
            await _noteRepository.AddAsync(note, token);
        }
    }
}
