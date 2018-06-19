using System;
using System.Linq;
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
    public class CreateAnswerCommandHandler : ICommandHandlerAsync<CreateAnswerCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Answer> _answerRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;
        private readonly IServiceBusProvider _blockChainProvider;
        private readonly IBlockChainErc20Service _erc20;

        public CreateAnswerCommandHandler(IRepository<Question> questionRepository, IRepository<Answer> answerRepository, IRepository<User> userRepository, IBlobProvider<QuestionAnswerContainer> blobProvider, IServiceBusProvider blockChainProvider, IBlockChainErc20Service erc20)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _userRepository = userRepository;
            _blobProvider = blobProvider;
            _blockChainProvider = blockChainProvider;
            _erc20 = erc20;
        }

        public async Task HandleAsync(CreateAnswerCommand message, CancellationToken token)
        {
            var user = await _userRepository.GetAsync(message.UserId, token).ConfigureAwait(false);
            var question = await _questionRepository.GetAsync(message.QuestionId, token).ConfigureAwait(false);
            if (user.Id == question.User.Id)
            {
                throw new InvalidOperationException("user cannot answer himself");
            }
            var answer = new Answer(question, message.Text, message.Files?.Count() ?? 0, user);
            await _answerRepository.SaveAsync(answer, token).ConfigureAwait(false);

            var id = answer.Id;
            var l = message.Files?.Select(file => _blobProvider.MoveAsync(file, $"question/{question.Id}/answer/{id}", token)) ?? Enumerable.Empty<Task>();

            var blockChainTask = _blockChainProvider.InsertMessageAsync(new BlockChainSubmitAnswer(question.Id, id, _erc20.GetAddress(user.PrivateKey)), token);
            await Task.WhenAll(l.Union(new [] { blockChainTask })).ConfigureAwait(true);        }
    }
}