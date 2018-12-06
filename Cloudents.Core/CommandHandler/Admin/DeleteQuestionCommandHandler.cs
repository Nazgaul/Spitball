using Cloudents.Core.Attributes;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    [AdminCommandHandler]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Transaction> _transactionRepository;


        public DeleteQuestionCommandHandler(IRepository<Question> questionRepository, IRepository<Transaction> transactionRepository)
        {
            _questionRepository = questionRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(DeleteQuestionCommand message, CancellationToken token)
        {
            var question = await _questionRepository.GetAsync(message.QuestionId, token);
            if (question == null)
            {
                return;
            }
            if (question is QuestionDeleted)
            {
                return;
            }
            await DeleteQuestionAsync(question, token);
        }

        internal async Task DeleteQuestionAsync(Question question, CancellationToken token)
        {
            foreach (var transaction in question.Transactions)
            {
                await _transactionRepository.DeleteAsync(transaction, token);
            }

            question.Events.Add(new QuestionDeletedEvent(question));
            question.Events.Add(new QuestionDeletedAdminEvent(question));
            await _questionRepository.DeleteAsync(question, token);
        }
    }
}