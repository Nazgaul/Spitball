﻿using Cloudents.Core.Command;
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

            float condition = Math.Max(DateTime.UtcNow.Subtract(answer.Created).Seconds, 1);

            const int fraudTime = TimeConst.Minute * 8;
            if (condition < fraudTime)
            {
                var factor = fraudTime / condition;

                question.User.FraudScore += (int)factor * 5;

                await _userRepository.UpdateAsync(question.User, token);
            }

            var t1 = _questionRepository.UpdateAsync(question, token);
            await Task.WhenAll(t1/*, t2*/).ConfigureAwait(true);
        }
    }
}