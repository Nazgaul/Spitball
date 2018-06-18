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
    public class CreateQuestionCommandHandler : ICommandHandlerAsync<CreateQuestionCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<QuestionSubject> _questionSubjectRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;
        private readonly IServiceBusProvider _blockChainProvider;
        private readonly IBlockChainErc20Service _blockChain;

        public CreateQuestionCommandHandler(IRepository<Question> questionRepository, IRepository<QuestionSubject> questionSubjectRepository, IRepository<User> userRepository, IBlobProvider<QuestionAnswerContainer> blobProvider, IServiceBusProvider blockChainProvider, IBlockChainErc20Service blockChain)
        {
            _questionRepository = questionRepository;
            _questionSubjectRepository = questionSubjectRepository;
            _userRepository = userRepository;
            _blobProvider = blobProvider;
            _blockChainProvider = blockChainProvider;
            _blockChain = blockChain;
        }

        public async Task HandleAsync(CreateQuestionCommand message, CancellationToken token)
        {
            var user = await _userRepository.GetAsync(message.UserId, token).ConfigureAwait(false);
            var subject = await _questionSubjectRepository.GetAsync(message.SubjectId, token).ConfigureAwait(false);
            var question = new Question(subject, message.Text, message.Price, message.Files?.Count() ?? 0, user);
            await _questionRepository.AddAsync(question, token).ConfigureAwait(false);
            var id = question.Id;

            var p = _blockChainProvider.InsertMessageAsync(new BlockChainSubmitQuestion(id, message.Price, _blockChain.GetAddress(user.PrivateKey)), token);

            var l = message.Files?.Select(file => _blobProvider.MoveAsync(file, $"question/{id}", token)) ?? Enumerable.Empty<Task>();
            await Task.WhenAll(l.Union(new[] { p })).ConfigureAwait(true);
        }
    }
}
