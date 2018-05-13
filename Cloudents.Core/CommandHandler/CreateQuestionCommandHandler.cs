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

        public CreateQuestionCommandHandler(IRepository<Question> questionRepository, IRepository<QuestionSubject> questionSubjectRepository)
        {
            _questionRepository = questionRepository;
            _questionSubjectRepository = questionSubjectRepository;
        }

        public async Task HandleAsync(CreateQuestionCommand message, CancellationToken token)
        {
            var subject = await _questionSubjectRepository.LoadAsync(message.SubjectId, token).ConfigureAwait(false);
            var question = new Question(subject, message.Text, message.Price, 0);
            await _questionRepository.SaveAsync(question,token).ConfigureAwait(false);
        }
    }
}
