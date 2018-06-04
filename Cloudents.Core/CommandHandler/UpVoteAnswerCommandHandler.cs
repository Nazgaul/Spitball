using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    public class UpVoteAnswerCommandHandler : ICommandHandlerAsync<UpVoteAnswerCommand>
    {
        private readonly IRepository<Answer> _repository;
        private readonly IRepository<User> _userRepository;
        private readonly IBlockChainQAndAContract _blockChainProvider;

        public UpVoteAnswerCommandHandler(IRepository<Answer> repository, IBlockChainQAndAContract blockChainProvider, IRepository<User> userRepository)
        {
            _repository = repository;
            _blockChainProvider = blockChainProvider;
            _userRepository = userRepository;
        }

        public async Task HandleAsync(UpVoteAnswerCommand message, CancellationToken token)
        {
            var answer = await _repository.LoadAsync(message.Id, token).ConfigureAwait(false);
            var question = answer.Question;
            var user = await _userRepository.LoadAsync(message.UserId, token).ConfigureAwait(false);
            await _blockChainProvider.UpVoteAsync(user.PublicKey, question.Id, message.Id, token).ConfigureAwait(false);

            answer.UpVote++;
            await _repository.SaveAsync(answer, token).ConfigureAwait(true);
        }
    }
}