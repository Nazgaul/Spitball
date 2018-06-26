using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class CreateAnswerCommandHandler : ICommandHandler<CreateAnswerCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Answer> _answerRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;
        //private readonly ITransactionRepository _transactionRepository;
        //private readonly IBlockChainErc20Service _erc20;

        public CreateAnswerCommandHandler(IRepository<Question> questionRepository, 
            IRepository<Answer> answerRepository, IRepository<User> userRepository,
            IBlobProvider<QuestionAnswerContainer> blobProvider)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _userRepository = userRepository;
            _blobProvider = blobProvider;
        }

        public async Task ExecuteAsync(CreateAnswerCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token).ConfigureAwait(false);
            var question = await _questionRepository.LoadAsync(message.QuestionId, token).ConfigureAwait(false);
            if (user.Id == question.User.Id)
            {
                throw new InvalidOperationException("user cannot answer himself");
            }
            var answer = new Answer(question, message.Text, message.Files?.Count() ?? 0, user);
            await _answerRepository.AddAsync(answer, token).ConfigureAwait(false);

            var id = answer.Id;
            //TODO: not right
            var l = message.Files?.Select(file => _blobProvider.MoveAsync(file, $"question/{question.Id}/answer/{id}", token)) ?? Enumerable.Empty<Task>();

            //var t = Transaction.AnswerCreateTransaction(answer);
            //await _transactionRepository.AddAsync(t, token);

            //var blockChainTask = _blockChainProvider.InsertMessageAsync(new BlockChainSubmitAnswer(question.Id, id, _erc20.GetAddress(user.PrivateKey)), token);
            await Task.WhenAll(l/*.Union(new [] { blockChainTask })*/).ConfigureAwait(true);        }
    }
}