using Cloudents.Core.Command.Admin;
using Cloudents.Domain.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Domain.Enums;

namespace Cloudents.Core.CommandHandler.Admin
{
    public class ApproveAnswerCommandHandler : ICommandHandler<ApproveAnswerCommand>
    {
        private readonly IRepository<Answer> _answerRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IEventStore _eventStore;

        public ApproveAnswerCommandHandler(IRepository<Answer> answerRepository, IRepository<Question> questionRepository, IEventStore eventStore)
        {
            _answerRepository = answerRepository;
            _questionRepository = questionRepository;
            _eventStore = eventStore;
        }

        public async Task ExecuteAsync(ApproveAnswerCommand message, CancellationToken token)
        {
            foreach (var answerId in message.AnswerIds)
            {
                var answer = await _answerRepository.LoadAsync(answerId, token);
                answer.Item.State = ItemState.Ok;
                answer.Question.AnswerCount++;
                await _questionRepository.UpdateAsync(answer.Question, token);
                _eventStore.Add(new AnswerCreatedEvent(answer));
                await _answerRepository.UpdateAsync(answer, token);
            }

        }
    }
}
