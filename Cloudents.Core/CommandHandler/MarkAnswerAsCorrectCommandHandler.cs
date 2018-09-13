using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class MarkAnswerAsCorrectCommandHandler : ICommandHandler<MarkAnswerAsCorrectCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Answer> _answerRepository;
        //private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<User> _userRepository;

        public MarkAnswerAsCorrectCommandHandler(IRepository<Question> questionRepository,
            IRepository<Answer> answerRepository/*, IEventPublisher eventPublisher*/, IRepository<User> userRepository)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            // _eventPublisher = eventPublisher;
            _userRepository = userRepository;
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


            if (System.DateTime.Now.Subtract(answer.Created).Minutes < 8)
            {
                var user = await _userRepository.LoadAsync(question.User.Id, token).ConfigureAwait(false);
                user.FraudScore += 5;
                if (System.DateTime.Now.Subtract(answer.Created).Minutes < 4)
                    user.FraudScore += 10;
                await _userRepository.UpdateAsync(user, default);
            }

            var t1 = _questionRepository.UpdateAsync(question, token);
           // var t2 = _eventPublisher.PublishAsync(new MarkAsCorrectEvent(answer.Id), token);
            //var t2 = _serviceBusProvider.InsertMessageAsync(new AnswerCorrectEmail(answer.User.Email, answer.Question.Text, answer.Text, _urlBuilder.WalletEndPoint, answer.Question.Price), token);
            await Task.WhenAll(t1/*, t2*/).ConfigureAwait(true);
        }
    }
}