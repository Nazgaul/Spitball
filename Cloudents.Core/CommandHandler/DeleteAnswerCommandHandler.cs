using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Domain.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Domain.Enums;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class DeleteAnswerCommandHandler : ICommandHandler<DeleteAnswerCommand>
    {
        private readonly IRepository<Answer> _repository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IEventStore _eventStore;

        public DeleteAnswerCommandHandler(IRepository<Answer> repository, IRepository<Question> questionRepository, IEventStore eventStore)
        {
            _repository = repository;
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

            if (answer.Item.State != ItemState.Ok)
            {
                throw new ArgumentException("answer doesn't exits");

            }

            if (answer.User.Id != message.UserId)
            {
                throw new InvalidOperationException("user is not the one who wrote the answer");
            }
            if (answer.Question.CorrectAnswer?.Id == message.Id)
            {
                throw new ArgumentException("this is answer is correct answer");
            }
            answer.Question.AnswerCount--;
           // await _questionRepository.UpdateAsync(answer.Question, token);

            _eventStore.Add(new AnswerDeletedEvent(answer));
            
            await _repository.DeleteAsync(answer, token);
        }
    }
}