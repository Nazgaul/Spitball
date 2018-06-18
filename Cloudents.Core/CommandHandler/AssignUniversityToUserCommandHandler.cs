using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class AssignUniversityToUserCommandHandler : ICommandHandlerAsync<AssignUniversityToUserCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<University> _universityRepository;

        public AssignUniversityToUserCommandHandler(IRepository<User> userRepository, IRepository<University> universityRepository)
        {
            _userRepository = userRepository;
            _universityRepository = universityRepository;
        }

        public async Task HandleAsync(AssignUniversityToUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.GetAsync(message.UserId, token).ConfigureAwait(false);
            var university = await _universityRepository.GetAsync(message.UniversityId, token).ConfigureAwait(false);

            user.University = university;
            await _userRepository.AddAsync(user, token).ConfigureAwait(false);
        }
    }
}