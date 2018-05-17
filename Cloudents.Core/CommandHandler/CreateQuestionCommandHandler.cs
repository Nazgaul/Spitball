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

        public CreateQuestionCommandHandler(IRepository<Question> questionRepository, IRepository<QuestionSubject> questionSubjectRepository, IRepository<User> userRepository, IBlobProvider<QuestionAnswerContainer> blobProvider)
        {
            _questionRepository = questionRepository;
            _questionSubjectRepository = questionSubjectRepository;
            _userRepository = userRepository;
            _blobProvider = blobProvider;
        }

        public async Task HandleAsync(CreateQuestionCommand message, CancellationToken token)
        {
            var subject = await _questionSubjectRepository.LoadAsync(message.SubjectId, token).ConfigureAwait(false);
            var user = await _userRepository.LoadAsync(message.UserId, token).ConfigureAwait(false);
            var question = new Question(subject, message.Text, message.Price, message.Files.Count(), user);
            await _questionRepository.SaveAsync(question, token).ConfigureAwait(false);
            var id = question.Id;

            var l = message.Files.Select(file => _blobProvider.MoveAsync(file, $"{user.Id}/{id}", token));

            await Task.WhenAll(l).ConfigureAwait(false);
        }
    }
}
