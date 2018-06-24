using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Core.Storage;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class MarkAnswerAsCorrectCommandHandler : ICommandHandler<MarkAnswerAsCorrectCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Answer> _answerRepository;
        private readonly IServiceBusProvider _blockChainProvider;
        private readonly IBlockChainErc20Service _blockChain;

        public MarkAnswerAsCorrectCommandHandler(IRepository<Question> questionRepository, IServiceBusProvider blockChainProvider,
            IRepository<Answer> answerRepository, IBlockChainErc20Service blockChain)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _blockChain = blockChain;
            _blockChainProvider = blockChainProvider;
        }

        public async Task ExecuteAsync(MarkAnswerAsCorrectCommand message, CancellationToken token)
        {
            var answer = await _answerRepository.LoadAsync(message.AnswerId, token).ConfigureAwait(true); //false will raise an exception
            var question = answer.Question;
            if (question.User.Id != message.UserId)
            {
                throw new ApplicationException("only owner can perform this task");
            }
            if (answer.Question.Id != question.Id)
            {
                throw new ApplicationException("answer is not connected to question");
            }
            question.MarkAnswerAsCorrect(answer);
            await _questionRepository.UpdateAsync(question, token).ConfigureAwait(false);

            await _blockChainProvider.InsertMessageAsync(new BlockChainMarkQuestionAsCorrect(_blockChain.GetAddress(question.User.PrivateKey), _blockChain.GetAddress(answer.User.PrivateKey), question.Id, answer.Id), token).ConfigureAwait(true);
        }
    }
}