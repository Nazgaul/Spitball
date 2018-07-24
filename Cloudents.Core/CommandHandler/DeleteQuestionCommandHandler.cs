using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
    {
        private readonly IRepository<Question> _repository;

        public DeleteQuestionCommandHandler(IRepository<Question> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(DeleteQuestionCommand message, CancellationToken token)
        {
            var question = await _repository.GetAsync(message.Id, token).ConfigureAwait(false); // no point in load next line will do a query
            if (question == null)
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

            question.QuestionDeleteTransaction();
            await _repository.DeleteAsync(question, token).ConfigureAwait(false);
        }
    }
}