using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Command.CommandHandler
{
    [UsedImplicitly]
    public class AssignUniversityToUserCommandHandler : ICommandHandler<AssignUniversityToUserCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IUniversityRepository _universityRepository;

        public AssignUniversityToUserCommandHandler(IRepository<RegularUser> userRepository,
            IUniversityRepository universityRepository)
        {
            _userRepository = userRepository;
            _universityRepository = universityRepository;
        }

        public async Task ExecuteAsync(AssignUniversityToUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var university = await _universityRepository.GetUniversityByNameAsync(message.UniversityName, token);
            if (university == null)
            {
                university = new University(message.UniversityName, user.Country);
                await _universityRepository.AddAsync(university, token);
            }

            //if (user.University == null)
            //{
            //    user.AwardMoney(AwardsTransaction.University);
            //}
            user.University = university;
            await _userRepository.UpdateAsync(user, token);
        }
    }
}