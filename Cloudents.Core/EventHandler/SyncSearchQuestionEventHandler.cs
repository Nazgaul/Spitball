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
                QuestionId = dbQuestion.Id,
                DateTime = dbQuestion.Updated,
                Text = dbQuestion.Text,
                Country = dbQuestion.User.Country,
                Language = dbQuestion.Language?.TwoLetterISOLanguageName,
                Subject = dbQuestion.Subject,
                Filter = QuestionFilter.Unanswered
            };


            //var question = new Question(dbQuestion.Id, dbQuestion.Updated, dbQuestion.Text, dbQuestion.User.Country,
            //    dbQuestion.Language?.TwoLetterISOLanguageName,
            //    dbQuestion.Subject, QuestionFilter.Unanswered);
          
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(true, question), token);
        }

        public Task HandleAsync(MarkAsCorrectEvent eventMessage, CancellationToken token)
        {
            var question = new QuestionSearchDto
            {
                QuestionId = eventMessage.Answer.Question.Id,
                Filter = QuestionFilter.Sold,
            };
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(true, question), token);
        }

        public Task HandleAsync(QuestionDeletedEvent eventMessage, CancellationToken token)
        {
            var question = new QuestionSearchDto
            {
                QuestionId = eventMessage.Question.Id,
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
                QuestionId = eventMessage.Answer.Question.Id,
                Filter = QuestionFilter.Answered,
            };
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(true, question), token);
        }

        public Task HandleAsync(AnswerDeletedEvent eventMessage, CancellationToken token)
        {
            //if (eventMessage.Answer.Question.Answers.Count == 0)
            //{
            //    return Task.CompletedTask;
            //}
            var question = new QuestionSearchDto
            {
                QuestionId = eventMessage.Answer.Question.Id,
                Filter = QuestionFilter.Unanswered
            };
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(true, question), token);
        }

        public Task HandleAsync(QuestionDeletedAdminEvent eventMessage, CancellationToken token)
        {
            var question = new QuestionSearchDto
            {
                QuestionId = eventMessage.Question.Id,
            };
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(false, question), token);
        }
    }
}