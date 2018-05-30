using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    public class MarkAnswerAsCorrectCommandHandler : ICommandHandlerAsync<MarkAnswerAsCorrectCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Answer> _answerRepository;

        public MarkAnswerAsCorrectCommandHandler(IRepository<Question> questionRepository,
            IRepository<Answer> answerRepository)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
        }

        public async Task HandleAsync(MarkAnswerAsCorrectCommand message, CancellationToken token)
        {
            var answer = await _answerRepository.LoadAsync(message.AnswerId, token).ConfigureAwait(true); //false will raise an exception
            var question = answer.Question;
            //var question = await _questionRepository.LoadAsync(message.QuestionId, token).ConfigureAwait(false);
            if (question.User.Id != message.UserId)
            {
                throw new ApplicationException("only owner can perform this task");
            }

            //var answer = await _answerRepository.LoadAsync(message.AnswerId, token).ConfigureAwait(false);

            if (answer.Question.Id != question.Id)
            {
                throw new ApplicationException("answer is not connected to question");
            }
            question.CorrectAnswer = answer;
            await _questionRepository.SaveAsync(question, token).ConfigureAwait(false);
        }
    }
}