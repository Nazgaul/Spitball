using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    public class DeleteAnswerCommandHandler : ICommandHandler<DeleteAnswerCommand>
    {
        private readonly IRepository<Answer> _repository;
        private readonly ITransactionRepository _transactionRepository;

        public DeleteAnswerCommandHandler(IRepository<Answer> repository, ITransactionRepository transactionRepository)
        {
            _repository = repository;
            _transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(DeleteAnswerCommand message, CancellationToken token)
        {
            var answer = await _repository.LoadAsync(message.Id, token).ConfigureAwait(false);
            if (answer.User.Id != message.UserId)
            {
                throw new InvalidOperationException("user is not the one who wrote the answer");
            }

            var t = Transaction.AnswerDeleteTransaction(answer);
            await _transactionRepository.AddAsync(t, token);
            await _repository.DeleteAsync(answer, token).ConfigureAwait(false);
        }
    }
}