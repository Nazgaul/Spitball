using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
    {
        private readonly IRepository<Question> _repository;
        private readonly IRepository<Transaction> _transactionRepository;

        public DeleteQuestionCommandHandler(IRepository<Question> repository, IRepository<Transaction> transactionRepository)
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

            if (question.Answers.Count > 0)
            {
                throw new InvalidOperationException("cannot delete question with answers");
            }

            //question.Transactions = null;
            foreach (var transaction in question.Transactions)
            {
                transaction.Question = null;
                await _transactionRepository.UpdateAsync(transaction, token);
            }
            question.QuestionDeleteTransaction();
            question.Events.Add(new QuestionDeletedEvent(question));
            await _repository.DeleteAsync(question, token).ConfigureAwait(false);

        }
    }
}