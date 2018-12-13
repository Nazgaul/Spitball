using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
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
        private readonly IRepository<Transaction> _transactionRepository;


        public MarkAnswerAsCorrectCommandHandler(IRepository<Question> questionRepository,
            IRepository<Answer> answerRepository, IRepository<Transaction> transactionRepository)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(MarkAnswerAsCorrectCommand message, CancellationToken token)
        {
            var answer = await _answerRepository.LoadAsync(message.AnswerId, token); //false will raise an exception
            if (answer.Question.Item.State != ItemState.Ok)
            {
                throw new InvalidOperationException("only owner can perform this task");
            }

            var question = answer.Question;
            if (question.User.Id != message.QuestionUserId)
            {
                throw new InvalidOperationException("only owner can perform this task");
            }
            if (answer.Question.Id != question.Id)
            {
                throw new InvalidOperationException("answer is not connected to question");
            }

            if (question.CorrectAnswer != null)
            {
                throw new InvalidOperationException("Already have correct answer");
            }

            question.CorrectAnswer = answer;
            if (question.User is RegularUser questionUser)
            {
                var t1 = CorrectAnswer(TransactionType.Stake, question,
                    questionUser);
                var t2 = CorrectAnswer(TransactionType.Spent, question,
                    questionUser);
                var t3 = new Transaction(TransactionActionType.Awarded, TransactionType.Earned, ReputationAction.AcceptItemOwner, questionUser);
                await _transactionRepository.AddAsync(new[] { t1, t2, t3 }, token);
            }
            var tAnswer = CorrectAnswer(TransactionType.Earned, question, answer.User);
            var t4 = new Transaction(TransactionActionType.Awarded, TransactionType.Earned, ReputationAction.AcceptItemUser, answer.User);

            await _transactionRepository.AddAsync(new[] { tAnswer, t4 }, token);

            answer.Events.Add(new MarkAsCorrectEvent(answer));

            //TODO: need to put it as event
            // await FraudDetectionAsync(question, answer, token);
            await _questionRepository.UpdateAsync(question, token);
        }




        private static Transaction CorrectAnswer(TransactionType type, Question question,
            RegularUser user)
        {
            var price = question.Price;
            if (type == TransactionType.Spent)
            {
                price = -price;
            }
            return new Transaction(TransactionActionType.AnswerCorrect, type, price, user)
            {
                Question = question,
                Answer = question.CorrectAnswer
            };
        }


        //TODO: this is no good - we need to figure out how to change its location - this command handler should handle the fraud score

        //private async Task FraudDetectionAsync(Question question, Answer answer, CancellationToken token)
        //{
        //    float condition = Math.Max(DateTime.UtcNow.Subtract(answer.Created).Seconds, 1);
        //    const int fraudTime = TimeConst.Minute * 8;
        //    if (condition < fraudTime)
        //    {
        //        var factor = fraudTime / condition;
        //        question.User.FraudScore += (int)factor * 5;
        //        answer.User.FraudScore += (int)factor * 5;

        //        await _userRepository.UpdateAsync(question.User, token);
        //        await _userRepository.UpdateAsync(answer.User, token);
        //    }
        //}
    }

}