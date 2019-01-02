using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Command.CommandHandler
{
    [UsedImplicitly]
    public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
    {
        private readonly IRepository<Question> _repository;
        private readonly IRepository<Transaction> _transactionRepository;


        public DeleteQuestionCommandHandler(IRepository<Question> repository,
            IRepository<Transaction> transactionRepository)
        {
            _repository = repository;
            _transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(DeleteQuestionCommand message, CancellationToken token)
        {
            var question = await _repository.GetAsync(message.Id, token).ConfigureAwait(false); // no point in load next line will do a query
            if (question == null)
            {
                throw new ArgumentException("question doesn't exists");
            }

            if (question.State != ItemState.Ok)
            {
                throw new ArgumentException("question doesn't exists");

            }
            
            if (question.User.Id != message.UserId)
            {
                throw new InvalidOperationException("user is not the one who wrote the question");
            }

            if (question.Answers.Count(w=>w.State == ItemState.Ok) > 0)
            {
                throw new InvalidOperationException("cannot delete question with answers");
            }

            if (!(question.User.Actual is RegularUser user))
            {
                throw new InvalidOperationException("cannot delete fictive user");

            }

            foreach (var transaction in question.Transactions)
            {
                transaction.Question = null;
                await _transactionRepository.UpdateAsync(transaction, token);
            }

            var deleteQuestionTransaction = new Transaction(TransactionActionType.DeleteQuestion,
                TransactionType.Stake, question.Price, user);
            await _transactionRepository.AddAsync(deleteQuestionTransaction, token);

            //question.Events.Add(new QuestionDeletedEvent(question));
            await _repository.DeleteAsync(question, token);

        }
    }
}