using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler.Admin
{
    public class ApproveQuestionCommandHandler : ICommandHandler<ApproveQuestionCommand>
    {
        private readonly IRepository<QuestionPending> _questionRepository;

        public ApproveQuestionCommandHandler(IRepository<QuestionPending> questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task ExecuteAsync(ApproveQuestionCommand message, CancellationToken token)
        {
            foreach (var questionId in message.QuestionIds)
            {
                var question = await _questionRepository.LoadAsync(questionId, token);
                question.State = ItemState.Ok;
                question.Updated = DateTime.UtcNow;

                question.Events.Add(new QuestionCreatedEvent(question));
                await _questionRepository.UpdateAsync(question, token);
            }
          
        }
    }
}