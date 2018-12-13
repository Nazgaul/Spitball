﻿using Cloudents.Core.Attributes;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;

namespace Cloudents.Core.CommandHandler.Admin
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
            if (question.Item.State == ItemState.Deleted)
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
            foreach (var transaction in question.Transactions)
            {
                await _transactionRepository.DeleteAsync(transaction, token);
            }

            _eventStore.Add(new QuestionDeletedAdminEvent(question, user));
            //question.Events.Add(new QuestionRejectEvent(user));
            await _questionRepository.DeleteAsync(question, token);
        }
    }
}