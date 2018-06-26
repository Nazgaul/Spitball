using System;
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
    public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
    {
        private readonly IRepository<Question> _repository;
        private readonly ITransactionRepository _transactionRepository;


        public DeleteQuestionCommandHandler(IRepository<Question> repository, ITransactionRepository transactionRepository)
        {
            _repository = repository;
            _transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(DeleteQuestionCommand message, CancellationToken token)
        {
            var question = await _repository.LoadAsync(message.Id, token).ConfigureAwait(false);
            if (question.User.Id != message.UserId)
            {
                throw new InvalidOperationException("user is not the one who wrote the question");
            }

            if (question.Answers.Count > 0)
            {
                throw new InvalidOperationException("cannot delete question with answers");
            }

            var lastNode =  await _transactionRepository.GetLastNodeOfUserAsync(message.UserId, token);
            var t =  lastNode.AddTransaction(ActionType.DeleteQuestion, TransactionType.Stake, question.Price);
            await _transactionRepository.AddAsync(t,token);
            await _repository.DeleteAsync(question, token).ConfigureAwait(false);
        }
    }
}