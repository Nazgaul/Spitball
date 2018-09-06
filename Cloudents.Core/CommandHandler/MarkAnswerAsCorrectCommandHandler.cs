using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class MarkAnswerAsCorrectCommandHandler : ICommandHandler<MarkAnswerAsCorrectCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Answer> _answerRepository;
        private readonly IRepository<User> _userRepository;

        public MarkAnswerAsCorrectCommandHandler(IRepository<Question> questionRepository,
            IRepository<Answer> answerRepository, IRepository<User> userRepository)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
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


            if (DateTime.Now.Subtract(answer.Created).Minutes < 8)
            {
                var user = question.User;
                user.FraudScore++;
                if (DateTime.Now.Subtract(answer.Created).Minutes < 4)
                    user.FraudScore++;
                await _userRepository.UpdateAsync(user, default);
            }

            await _questionRepository.UpdateAsync(question, token);
        }
    }
}