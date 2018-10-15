using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler
{
    public class CreateUniversityCommandHandler : ICommandHandler<CreateUniversityCommand>
    {
        private readonly IUniversityRepository _universityRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ICommandHandler<AssignUniversityToUserCommand> _assignUserCommandHandler;


        public CreateUniversityCommandHandler(IUniversityRepository universityRepository, IRepository<User> userRepository, ICommandHandler<AssignUniversityToUserCommand> assignUserCommandHandler)
        {
            _universityRepository = universityRepository;
            _userRepository = userRepository;
            _assignUserCommandHandler = assignUserCommandHandler;
        }

        public async Task ExecuteAsync(CreateUniversityCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);

            var universities = await _universityRepository.GetUniversityByNameAsync(message.Name, user.Country, token);
            University university;
            if (universities.Count > 0)
            {
                university = universities[0];
            }
            else
            {
                university = new University(message.Name, user.Country);
                await _universityRepository.AddAsync(university, token);
            }


            user.University = university;
            await _userRepository.UpdateAsync(user, token).ConfigureAwait(false);
            //var assignCommand = new AssignUniversityToUserCommand(message.UserId, university.Id);
            //await _assignUserCommandHandler.ExecuteAsync(assignCommand, token);
        }
    }
}