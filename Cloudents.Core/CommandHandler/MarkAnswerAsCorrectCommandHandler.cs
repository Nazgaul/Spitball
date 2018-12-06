using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class MarkAnswerAsCorrectCommandHandler : ICommandHandler<MarkAnswerAsCorrectCommand>
    {
        private readonly IRepository<QuestionApproved> _questionRepository;
        private readonly IRepository<AnswerApproved> _answerRepository;
        private readonly IRepository<User> _userRepository;
        

        public MarkAnswerAsCorrectCommandHandler(IRepository<QuestionApproved> questionRepository,
            IRepository<AnswerApproved> answerRepository, IRepository<User> userRepository)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(MarkAnswerAsCorrectCommand message, CancellationToken token)
        {
            var answer = await _answerRepository.LoadAsync(message.AnswerId, token).ConfigureAwait(true); //false will raise an exception
            if (!(answer.Question is QuestionApproved question))
            {
                throw new InvalidOperationException("only owner can perform this task");

            }
            if (question.User.Id != message.QuestionUserId)
            {
                throw new InvalidOperationException("only owner can perform this task");
            }
            if (answer.Question.Id != question.Id)
            {
                throw new InvalidOperationException("answer is not connected to question");
            }
            question.MarkAnswerAsCorrect(answer);

            float condition = Math.Max(DateTime.UtcNow.Subtract(answer.Created).Seconds, 1);

            
            //TODO: this is no good - we need to figure out how to change its location - this command handler should handle the fraud score
            const int fraudTime = TimeConst.Minute * 8;
            if (condition < fraudTime)
            {
                var factor = fraudTime / condition;
                question.User.FraudScore += (int)factor * 5;
                answer.User.FraudScore += (int) factor * 5;
                
                await _userRepository.UpdateAsync(question.User, token);
                await _userRepository.UpdateAsync(answer.User, token);
            }
            await _questionRepository.UpdateAsync(question, token);
         
        }
    }

}