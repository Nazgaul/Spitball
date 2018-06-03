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
        private readonly IBlockChainQAndAContract _blockChainProvider;

        public UpVoteAnswerCommandHandler(IRepository<Answer> repository, IBlockChainQAndAContract blockChainProvider)
        {
            _repository = repository;
            _blockChainProvider = blockChainProvider;
        }

        public async Task HandleAsync(UpVoteAnswerCommand message, CancellationToken token)
        {
            var answer = await _repository.LoadAsync(message.Id, token).ConfigureAwait(false);
            answer.UpVote++;
            await _repository.SaveAsync(answer, token).ConfigureAwait(false);

            var question = answer.Question;
           // var p = _blockChainProvider.UpVoteAsync(, question.Id, answer.Id, token);
           //TODO: UpVoter Address
           // await Task.WhenAll(p).ConfigureAwait(false);
        }
    }
}