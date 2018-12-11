using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class AssignUniversityToUserCommandHandler : ICommandHandler<AssignUniversityToUserCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IUniversityRepository _universityRepository;
        private readonly ITransactionRepository _transactionRepository;

        public AssignUniversityToUserCommandHandler(IRepository<RegularUser> userRepository, IUniversityRepository universityRepository, ITransactionRepository transactionRepository)
        {
            _userRepository = userRepository;
            _universityRepository = universityRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(AssignUniversityToUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token).ConfigureAwait(false);
            var country = message.Country ?? user.Country;
            var university = await _universityRepository.GetUniversityByNameAsync(message.UniversityName, country, token);
            if (university == null)
            {
                university = new University(message.UniversityName, user.Country);
                await _universityRepository.AddAsync(university, token);
            }

            if (user.University == null)
            {
                var t = new Transaction(TransactionActionType.Awarded, TransactionType.Earned,ReputationAction.University, user);
                await _transactionRepository.AddAsync(t, token);
            }
            user.University = university;
            await _userRepository.UpdateAsync(user, token).ConfigureAwait(false);
        }
    }
}