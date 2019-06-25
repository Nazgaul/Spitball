using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Ioc resolve")]
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<University> _universityRepository;

        public CreateUserCommandHandler(IRegularUserRepository userRepository, IRepository<University> universityRepository)
        {
            _userRepository = userRepository;
            _universityRepository = universityRepository;
        }

        public async Task ExecuteAsync(CreateUserCommand message, CancellationToken token)
        {
            if (message.UniversityId.HasValue)
            {
                var university = await _universityRepository.LoadAsync(message.UniversityId.Value,token);
                message.User.SetUniversity(university);

            }
            await _userRepository.AddAsync(message.User, token);
        }
    }
}