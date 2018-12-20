using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Attributes;
using Cloudents.Application.Command.Admin;
using Cloudents.Application.Event;
using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;
using Cloudents.Domain.Enums;

namespace Cloudents.Application.CommandHandler.Admin
{
    [AdminCommandHandler]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class DeleteAnswerCommandHandler : ICommandHandler<DeleteAnswerCommand>
    {
        private readonly IRepository<Answer> _repository;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IEventStore _eventStore;

        public DeleteAnswerCommandHandler(IRepository<Answer> repository,
            IRepository<Transaction> transactionRepository, IRepository<Question> questionRepository, IEventStore eventStore)
        {
            _repository = repository;
            _transactionRepository = transactionRepository;
            _questionRepository = questionRepository;
            _eventStore = eventStore;
        }

        public async Task ExecuteAsync(DeleteAnswerCommand message, CancellationToken token)
        {
            var answer = await _repository.GetAsync(message.Id, token); //no point in load since next line will do query
            if (answer == null)
            {
                throw new ArgumentException("answer doesn't exits");
            }

            if (answer.Item.State == ItemState.Deleted)
            {
                throw new ArgumentException("answer doesn't exits");
            }
            
            await DeleteAnswerAsync(answer, token);
        }

        internal async Task DeleteAnswerAsync(Answer answer, CancellationToken token)
        {
                foreach (var transaction in answer.TransactionsReadOnly)
                {
                    await _transactionRepository.DeleteAsync(transaction, token);
                }
            
            
            _eventStore.Add(new AnswerDeletedEvent(answer));

            answer.Question.AnswerCount--;
            if (answer.Question.CorrectAnswer != null)
            {
                if (answer.Id == answer.Question.CorrectAnswer.Id)
                {
                    answer.Question.CorrectAnswer = null;
                }
            }
            await _questionRepository.UpdateAsync(answer.Question, token);
            await _repository.DeleteAsync(answer, token).ConfigureAwait(false);
        }
    }
}
