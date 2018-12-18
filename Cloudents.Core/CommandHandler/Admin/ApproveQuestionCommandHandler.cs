using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Domain.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Domain.Enums;

namespace Cloudents.Core.CommandHandler.Admin
{
    public class ApproveQuestionCommandHandler : ICommandHandler<ApproveQuestionCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IEventStore _eventStore;


        public ApproveQuestionCommandHandler(IRepository<Question> questionRepository, IEventStore eventStore)
        {
            _questionRepository = questionRepository;
            _eventStore = eventStore;
        }

        public async Task ExecuteAsync(ApproveQuestionCommand message, CancellationToken token)
        {
            foreach (var questionId in message.QuestionIds)
            {
                var question = await _questionRepository.LoadAsync(questionId, token);
                question.Item.State = ItemState.Ok;
                question.Updated = DateTime.UtcNow;

                _eventStore.Add(new QuestionCreatedEvent(question));
                await _questionRepository.UpdateAsync(question, token);
            }
          
        }
    }
}