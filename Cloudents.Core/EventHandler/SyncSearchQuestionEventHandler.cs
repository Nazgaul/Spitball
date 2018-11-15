using Cloudents.Core.Entities.Search;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class SyncSearchQuestionEventHandler : 
        IEventHandler<MarkAsCorrectEvent>,
        IEventHandler<QuestionCreatedEvent>,
        IEventHandler<QuestionDeletedEvent>,
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
            var question = new Question(dbQuestion.Id, dbQuestion.Created, dbQuestion.Text, dbQuestion.User.Country,
                dbQuestion.Language?.TwoLetterISOLanguageName,
                dbQuestion.Subject, QuestionFilter.Unanswered);
          
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(true, question), token);
        }

        public Task HandleAsync(MarkAsCorrectEvent eventMessage, CancellationToken token)
        {
            var question = new Question
            {
                Id = eventMessage.Answer.Question.Id.ToString(),
                State = QuestionFilter.Sold,
            };
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(true, question), token);
        }

        public Task HandleAsync(QuestionDeletedEvent eventMessage, CancellationToken token)
        {
            var question = new Question
            {
                Id = eventMessage.Question.Id.ToString(),
            };
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(false, question), token);
        }

        public Task HandleAsync(AnswerCreatedEvent eventMessage, CancellationToken token)
        {
            if (eventMessage.Answer.Question.Answers.Count > 1)
            {
                return Task.CompletedTask;
            }
            var question = new Question
            {
                Id = eventMessage.Answer.Question.Id.ToString(),
                State = QuestionFilter.Answered,
            };
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(true, question), token);
        }

        public Task HandleAsync(AnswerDeletedEvent eventMessage, CancellationToken token)
        {
            if (eventMessage.Answer.Question.Answers.Count == 0)
            {
                return Task.CompletedTask;
            }
            var question = new Question
            {
                Id = eventMessage.Answer.Question.Id.ToString(),
                State = QuestionFilter.Unanswered
            };
            return _queueProvider.InsertMessageAsync(new QuestionSearchMessage(true, question), token);
        }
    }
}