using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    public class DeleteAnswerCommandHandler : ICommandHandlerAsync<DeleteAnswerCommand>
    {
        private readonly IRepository<Answer> _repository;

        public DeleteAnswerCommandHandler(IRepository<Answer> repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(DeleteAnswerCommand message, CancellationToken token)
        {
            var answer = await _repository.LoadAsync(message.Id, token).ConfigureAwait(false);
            if (answer.User.Id != message.UserId)
            {
                throw new InvalidOperationException("user is not the one who wrote the answer");
            }

            await _repository.DeleteAsync(answer, token).ConfigureAwait(false);
        }
    }
}