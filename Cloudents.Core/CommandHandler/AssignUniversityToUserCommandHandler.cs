using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class AssignUniversityToUserCommandHandler : ICommandHandler<AssignUniversityToUserCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IUniversityRepository _universityRepository;

        public AssignUniversityToUserCommandHandler(IRepository<User> userRepository, IUniversityRepository universityRepository)
        {
            _userRepository = userRepository;
            _universityRepository = universityRepository;
        }

        public async Task ExecuteAsync(AssignUniversityToUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token).ConfigureAwait(false);
            var university = await _universityRepository.GetUniversityByNameAsync(message.UniversityName, user.Country, token);
            if (university == null)
            {
                university = new University(message.UniversityName, user.Country);
                await _universityRepository.AddAsync(university, token);
            }

            user.University = university;
            await _userRepository.UpdateAsync(user, token).ConfigureAwait(false);
        }
    }
}