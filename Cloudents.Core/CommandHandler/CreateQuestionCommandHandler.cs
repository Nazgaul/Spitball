using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class CreateQuestionCommandHandler : ICommandHandlerAsync<CreateQuestionCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<QuestionSubject> _questionSubjectRepository;
        private readonly IRepository<User> _userRepository;

        public CreateQuestionCommandHandler(IRepository<Question> questionRepository, IRepository<QuestionSubject> questionSubjectRepository, IRepository<User> userRepository)
        {
            _questionRepository = questionRepository;
            _questionSubjectRepository = questionSubjectRepository;
            _userRepository = userRepository;
        }

        public async Task HandleAsync(CreateQuestionCommand message, CancellationToken token)
        {
            var subject = await _questionSubjectRepository.LoadAsync(message.SubjectId, token).ConfigureAwait(false);
            var user = await _userRepository.LoadAsync(message.UserId, token).ConfigureAwait(false);
            var question = new Question(subject, message.Text, message.Price, 0, user);
            await _questionRepository.SaveAsync(question, token).ConfigureAwait(false);
        }
    }
}
