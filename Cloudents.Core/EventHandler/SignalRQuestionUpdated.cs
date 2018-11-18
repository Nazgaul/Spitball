using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    public class SignalRQuestionUpdated : IEventHandler<MarkAsCorrectEvent>, 
        IEventHandler<AnswerCreatedEvent>, IEventHandler<AnswerDeletedEvent>
    {
        private readonly IQueueProvider _queueProvider;

        public SignalRQuestionUpdated(IQueueProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }

        private async Task Handle(Question question, CancellationToken token)
        {
            var user = new UserDto
            {
                Id = question.User.Id,
                Name = question.User.Name,
                Image = question.User.Image
            };
            var dto = new QuestionFeedDto(question.Id,
                question.Subject,
                question.Price,
                question.Text,
                question.Attachments,
                question.Answers.Count,
                user,
                question.Updated,
                question.Color,
                question.CorrectAnswer?.Id != null,
                question.Language);

            
            await _queueProvider.InsertMessageAsync(new SignalRMessage(SignalRType.Question, SignalRAction.Update, dto), token);
        }

        public Task HandleAsync(MarkAsCorrectEvent eventMessage, CancellationToken token)
        {
            return Handle(eventMessage.Answer.Question, token);
        }

        public Task HandleAsync(AnswerCreatedEvent eventMessage, CancellationToken token)
        {
            return Handle(eventMessage.Answer.Question, token);
        }

        public Task HandleAsync(AnswerDeletedEvent eventMessage, CancellationToken token)
        {
            return Handle(eventMessage.Answer.Question, token);
        }
    }
}
