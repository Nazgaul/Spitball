using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    [UsedImplicitly]
    public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
    {
        private readonly IRepository<Question> _repository;
        private readonly IRepository<User> _userRepository;


        public DeleteQuestionCommandHandler(IRepository<Question> repository,
            IRepository<User> userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(DeleteQuestionCommand message, CancellationToken token)
        {
            var question = await _repository.GetAsync(message.Id, token); // no point in load next line will do a query
            if (question == null)
            {
                throw new ArgumentException("question doesn't exists");
            }

            if (question.Status.State != ItemState.Ok)
            {
                throw new ArgumentException("question doesn't exists");

            }

            if (question.User.Id != message.UserId)
            {
                throw new InvalidOperationException("user is not the one who wrote the question");
            }

            if (question.Answers.Count(w => w.Status.State == ItemState.Ok) > 0)
            {
                throw new InvalidOperationException("cannot delete question with answers");
            }

            if (!(question.User.Actual is User user))
            {
                throw new InvalidOperationException("cannot delete fictive user");
            }

            //foreach (var transaction in question.Transactions)
            //{
            //    transaction.Question = null;
            //    await _transactionRepository.UpdateAsync(transaction, token);
            //}
            
            await _userRepository.UpdateAsync(user, token);
            await _repository.DeleteAsync(question, token);

        }
    }
}