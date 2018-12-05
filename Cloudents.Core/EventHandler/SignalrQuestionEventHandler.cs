using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
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
            IEventHandler<AnswerCreatedEvent>, IEventHandler<AnswerDeletedEvent>,
            IEventHandler<TransactionEvent>
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
            var dto = new
            {
                id = eventMessage.Question.Id
            };

            await _queueProvider.InsertMessageAsync(new SignalRMessage(SignalRType.Question, SignalRAction.Delete, dto), token);
        }

        public Task HandleAsync(MarkAsCorrectEvent eventMessage, CancellationToken token)
        {
            var question = eventMessage.Answer.Question;
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


            return _queueProvider.InsertMessageAsync(new SignalRMessage(SignalRType.Question, SignalRAction.Update, dto), token);
        }

        public Task HandleAsync(AnswerCreatedEvent eventMessage, CancellationToken token)
        {
            var user = new UserDto
            {
                Id = eventMessage.Answer.User.Id,
                Name = eventMessage.Answer.User.Name,
                Image = eventMessage.Answer.User.Image
            };
            var answerDto = new QuestionDetailAnswerDto
            {
                Create = eventMessage.Answer.Created,
                Files = null,
                Id = eventMessage.Answer.Id,
                User = user,
                Text = eventMessage.Answer.Text
            };
            var dto = new
            {
                questionId = eventMessage.Answer.Question.Id,
                answer = answerDto
            };

            return _queueProvider.InsertMessageAsync(new SignalRMessage(SignalRType.Answer, SignalRAction.Add, dto), token);
        }

        public Task HandleAsync(AnswerDeletedEvent eventMessage, CancellationToken token)
        {
            var dto = new
            {
                questionId = eventMessage.Answer.Question.Id,
                answer = new { id = eventMessage.Answer.Id}
            };

            return _queueProvider.InsertMessageAsync(new SignalRMessage(SignalRType.Answer, SignalRAction.Delete, dto), token);
        }

        public Task HandleAsync(TransactionEvent eventMessage, CancellationToken token)
        {
            var message = new SignalRMessage(SignalRType.User,
                SignalRAction.Update, new {balance = eventMessage.Transaction.User.Balance})
            {
                UserId = eventMessage.Transaction.User.Id
            };
            return _queueProvider.InsertMessageAsync
                (message, token);
        }
    }
}
