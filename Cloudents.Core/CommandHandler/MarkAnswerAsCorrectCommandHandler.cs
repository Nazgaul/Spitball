using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
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
        private readonly IServiceBusProvider _serviceBusProvider;

        public MarkAnswerAsCorrectCommandHandler(IRepository<Question> questionRepository,
            IRepository<Answer> answerRepository, IServiceBusProvider serviceBusProvider)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _serviceBusProvider = serviceBusProvider;
        }

        public async Task ExecuteAsync(MarkAnswerAsCorrectCommand message, CancellationToken token)
        {
            var answer = await _answerRepository.LoadAsync(message.AnswerId, token).ConfigureAwait(true); //false will raise an exception
            var question = answer.Question;
            if (question.User.Id != message.QuestionUserId)
            {
                throw new ApplicationException("only owner can perform this task");
            }
            if (answer.Question.Id != question.Id)
            {
                throw new ApplicationException("answer is not connected to question");
            }
            question.MarkAnswerAsCorrect(answer);

            var t1 = _questionRepository.UpdateAsync(question, token);
            var t2 = _serviceBusProvider.InsertMessageAsync(new AnswerCorrectEmail(answer.User.Email, answer.Question.Text, answer.Text, UrlConst.WalletEndPost, answer.Question.Price), token);
            await Task.WhenAll(t1, t2).ConfigureAwait(true);
        }
    }
}