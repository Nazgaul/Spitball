using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Universities
{
    public class CreateUniversityCommandHandler : ICommandHandler<CreateUniversityCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<University> _universityRepository;

        public CreateUniversityCommandHandler(IRepository<User> userRepository,
            IRepository<University> universityRepository)
        {
            _userRepository = userRepository;
            _universityRepository = universityRepository;
        }

        public async Task ExecuteAsync(CreateUniversityCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);

            var university = new University(message.UniversityName, user.Country);
            await _universityRepository.AddAsync(university, token);

            user.SetUniversity(university);
            await _userRepository.UpdateAsync(user, token);
        }
    }
}