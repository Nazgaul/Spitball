using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    public class SyncSearchQuestionEventHandler : 
        IEventHandler<MarkAsCorrectEvent>,
        IEventHandler<QuestionCreatedEvent>,
        IEventHandler<QuestionDeletedEvent>,
        IEventHandler<QuestionDeletedAdminEvent>,
        IEventHandler<AnswerCreatedEvent>,
        IEventHandler<AnswerDeletedEvent>
    {
        private readonly IQueueProvider _queueProvider;

        public SyncSearchQuestionEventHandler(IQueueProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }

        public Task HandleAsync(QuestionCreatedEvent eventMessage, CancellationToken token)
        {
            var dbQuestion = eventMessage.Question;
            var question = new QuestionSearchDto()
            {
                Id = dbQuestion.Id,
                DateTime = dbQuestion.Updated,
                Text = dbQuestion.Text,
                Country = dbQuestion.User.Country,
                Language = dbQuestion.Language?.TwoLetterISOLanguageName,
                Subject = dbQuestion.Subject,
                State = QuestionFilter.Unanswered,
                Course = dbQuestion.Course?.Id,
                UniversityName = dbQuestion.University?.Name
            };
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(true, question), token);
        }

        public Task HandleAsync(MarkAsCorrectEvent eventMessage, CancellationToken token)
        {
            var question = new QuestionSearchDto
            {
                Id = eventMessage.Answer.Question.Id,
                State = QuestionFilter.Sold,
            };
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(true, question), token);
        }

        public Task HandleAsync(QuestionDeletedEvent eventMessage, CancellationToken token)
        {
            var question = new QuestionSearchDto
            {
                Id = eventMessage.Question.Id,
            };
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(false, question), token);
        }

        public Task HandleAsync(AnswerCreatedEvent eventMessage, CancellationToken token)
        {
            //if (eventMessage.Answer.Question.Answers.Count > 1)
            //{
            //    return Task.CompletedTask;
            //}
            var question = new QuestionSearchDto
            {
                Id = eventMessage.Answer.Question.Id,
                State = QuestionFilter.Answered,
            };
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(true, question), token);
        }

        public Task HandleAsync(AnswerDeletedEvent eventMessage, CancellationToken token)
        {
            if (eventMessage.Answer.Question.Answers.Count(c=>c.Status.State == ItemState.Ok) == 0)
            {
                return Task.CompletedTask;
            }
            var question = new QuestionSearchDto
            {
                Id = eventMessage.Answer.Question.Id,
                State = QuestionFilter.Unanswered
            };
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(true, question), token);
        }

        public Task HandleAsync(QuestionDeletedAdminEvent eventMessage, CancellationToken token)
        {
            var question = new QuestionSearchDto
            {
                Id = eventMessage.Question.Id,
            };
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(false, question), token);
        }
    }
}