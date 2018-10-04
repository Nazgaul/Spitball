using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler
{
    public class CreateUniversityCommandHandler : ICommandHandler<CreateUniversityCommand>
    {
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ICommandHandler<Core.Command.AssignUniversityToUserCommand> _assignUserCommandHandler;


        public CreateUniversityCommandHandler(IRepository<University> universityRepository, IRepository<User> userRepository, ICommandHandler<AssignUniversityToUserCommand> assignUserCommandHandler)
        {
            _universityRepository = universityRepository;
            _userRepository = userRepository;
            _assignUserCommandHandler = assignUserCommandHandler;
        }

        public async Task ExecuteAsync(CreateUniversityCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var university = new University(message.Name, user.Country);
            await _universityRepository.AddAsync(university, token);

            var assignCommand = new AssignUniversityToUserCommand(message.UserId, university.Id);
            await _assignUserCommandHandler.ExecuteAsync(assignCommand, token);
        }
    }
}