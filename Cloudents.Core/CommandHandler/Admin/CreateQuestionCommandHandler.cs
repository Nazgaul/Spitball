using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Attributes;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler.Admin
{
    [AdminCommandHandler]
    public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand>
    {

        private readonly IUserRepository _userRepository;
        private readonly IRepository<Question> _questionRepository;

        public CreateQuestionCommandHandler(IUserRepository userRepository, IRepository<Question> questionRepository)
        {
            _userRepository = userRepository;
            _questionRepository = questionRepository;
        }


        public async Task ExecuteAsync(CreateQuestionCommand message, CancellationToken token)
        {
            var user = await _userRepository.GetRandomFictiveUserAsync(token);
            var question = new Question(message.SubjectId, message.Text, message.Price, 0, user, QuestionColor.Default)
            {
                State = QuestionState.Ok
            };
            await _questionRepository.AddAsync(question, token).ConfigureAwait(true);

        }
    }
}