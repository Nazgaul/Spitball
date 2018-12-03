using System;
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
    public class SignalrQuestionEventHandler 
        : IEventHandler<QuestionCreatedEvent>,
            IEventHandler<QuestionDeletedEvent>,
            IEventHandler<MarkAsCorrectEvent>,
            IEventHandler<AnswerCreatedEvent>, IEventHandler<AnswerDeletedEvent>
    {
        private readonly IServiceBusProvider _queueProvider;

        public SignalrQuestionEventHandler(IServiceBusProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }


        public async Task HandleAsync(QuestionCreatedEvent eventMessage, CancellationToken token)
        {
            var user = new UserDto
            {
                Id = eventMessage.Question.User.Id,
                Name = eventMessage.Question.User.Name,
                Image = eventMessage.Question.User.Image
            };
            var dto = new QuestionFeedDto(eventMessage.Question.Id,
                eventMessage.Question.Subject,
                eventMessage.Question.Price,
                eventMessage.Question.Text,
                eventMessage.Question.Attachments,
                0,
                user,
                DateTime.UtcNow,
                eventMessage.Question.Color,
                false,
                eventMessage.Question.Language);
            
            await _queueProvider.InsertMessageAsync(new SignalRMessage(SignalRType.Question, SignalRAction.Add, dto), token);
        }


        public async Task HandleAsync(QuestionDeletedEvent eventMessage, CancellationToken token)
        {
            var user = new UserDto
            {
                Id = eventMessage.Question.User.Id,
                Name = eventMessage.Question.User.Name,
                Image = eventMessage.Question.User.Image
            };
            var dto = new QuestionFeedDto(eventMessage.Question.Id,
                eventMessage.Question.Subject,
                eventMessage.Question.Price,
                eventMessage.Question.Text,
                eventMessage.Question.Attachments,
                0,
                user,
                eventMessage.Question.Updated,
                eventMessage.Question.Color,
                false,
                eventMessage.Question.Language);

            await _queueProvider.InsertMessageAsync(new SignalRMessage(SignalRType.Question, SignalRAction.Delete, dto), token);
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
