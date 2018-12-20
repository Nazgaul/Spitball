using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    [AdminCommandHandler]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IEventStore _eventStore;


        public DeleteQuestionCommandHandler(IRepository<Question> questionRepository, IRepository<Transaction> transactionRepository, IEventStore eventStore)
        {
            _questionRepository = questionRepository;
            _transactionRepository = transactionRepository;
            _eventStore = eventStore;
        }

        public async Task ExecuteAsync(DeleteQuestionCommand message, CancellationToken token)
        {
            var question = await _questionRepository.GetAsync(message.QuestionId, token);
            if (question == null)
            {
                return;
            }
            

            if (!(question.User is RegularUser user))
            {
                return;
            }
            await DeleteQuestionAsync(question, user, token);
        }

        internal async Task DeleteQuestionAsync(Question question,RegularUser user, CancellationToken token)
        {
            if (question.Item.State == ItemState.Deleted)
            {
                return;
            }
            foreach (var transaction in question.Transactions)
            {
                await _transactionRepository.DeleteAsync(transaction, token);
            }
            _eventStore.Add(new QuestionDeletedAdminEvent(question, user));
            await _questionRepository.DeleteAsync(question, token);
        }
    }
}