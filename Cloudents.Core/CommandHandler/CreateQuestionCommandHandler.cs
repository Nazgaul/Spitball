using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
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
        private readonly IBlockChainQAndAContract _blockChainProvider;

        public CreateQuestionCommandHandler(IRepository<Question> questionRepository, IRepository<QuestionSubject> questionSubjectRepository, IRepository<User> userRepository, IBlobProvider<QuestionAnswerContainer> blobProvider, IBlockChainQAndAContract blockChainProvider)
        {
            _questionRepository = questionRepository;
            _questionSubjectRepository = questionSubjectRepository;
            _userRepository = userRepository;
            _blobProvider = blobProvider;
            _blockChainProvider = blockChainProvider;
        }

        public async Task HandleAsync(CreateQuestionCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token).ConfigureAwait(false);
            var subject = await _questionSubjectRepository.LoadAsync(message.SubjectId, token).ConfigureAwait(false);
            var question = new Question(subject, message.Text, message.Price, message.Files?.Count() ?? 0, user);
            await _questionRepository.SaveAsync(question, token).ConfigureAwait(false);
            var id = question.Id;
            var p = _blockChainProvider.SubmitQuestionAsync(id, message.Price, user.PublicKey, token);

            var l = message.Files?.Select(file => _blobProvider.MoveAsync(file, $"question/{id}", token));

            await Task.WhenAll(l.Union(new[] { p })).ConfigureAwait(false);
        }
    }
}
