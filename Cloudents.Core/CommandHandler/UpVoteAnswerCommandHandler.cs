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

        public UpVoteAnswerCommandHandler(IRepository<Answer> repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(UpVoteAnswerCommand message, CancellationToken token)
        {
            var answer = await _repository.LoadAsync(message.AnswerId, token).ConfigureAwait(false);
            answer.UpVote++;
            await _repository.SaveAsync(answer, token).ConfigureAwait(false);
        }
    }
}