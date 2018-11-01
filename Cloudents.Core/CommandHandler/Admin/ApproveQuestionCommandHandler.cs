using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Attributes;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler.Admin
{
    [AdminCommandHandler]
    public class ApproveQuestionCommandHandler : ICommandHandler<ApproveQuestionCommand>
    {
        private readonly IRepository<Question> _questionRepository;

        public ApproveQuestionCommandHandler(IRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task ExecuteAsync(ApproveQuestionCommand message, CancellationToken token)
        {
            var question = await _questionRepository.LoadAsync(message.QuestionId, token);
            question.State = QuestionState.Ok;

            question.Events.Add(new QuestionCreatedEvent(question));
            await _questionRepository.UpdateAsync(question, token);
        }
    }
}