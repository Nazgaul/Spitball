using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class MarkAnswerAsCorrectCommandHandler : ICommandHandler<MarkAnswerAsCorrectCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Answer> _answerRepository;
        //private readonly IEventPublisher _eventPublisher;

        public MarkAnswerAsCorrectCommandHandler(IRepository<Question> questionRepository,
            IRepository<Answer> answerRepository/*, IEventPublisher eventPublisher*/)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
           // _eventPublisher = eventPublisher;
        }

        public async Task ExecuteAsync(MarkAnswerAsCorrectCommand message, CancellationToken token)
        {
            var answer = await _answerRepository.LoadAsync(message.AnswerId, token).ConfigureAwait(true); //false will raise an exception
            var question = answer.Question;
            if (question.User.Id != message.QuestionUserId)
            {
                throw new InvalidOperationException("only owner can perform this task");
            }
            if (answer.Question.Id != question.Id)
            {
                throw new InvalidOperationException("answer is not connected to question");
            }
            question.MarkAnswerAsCorrect(answer);

            var t1 = _questionRepository.UpdateAsync(question, token);
           // var t2 = _eventPublisher.PublishAsync(new MarkAsCorrectEvent(answer.Id), token);
            //var t2 = _serviceBusProvider.InsertMessageAsync(new AnswerCorrectEmail(answer.User.Email, answer.Question.Text, answer.Text, _urlBuilder.WalletEndPoint, answer.Question.Price), token);
            await Task.WhenAll(t1/*, t2*/).ConfigureAwait(true);
        }
    }
}