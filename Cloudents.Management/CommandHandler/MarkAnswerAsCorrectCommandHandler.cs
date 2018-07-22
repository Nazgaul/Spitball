using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Management.Command;
using JetBrains.Annotations;

namespace Cloudents.Management.CommandHandler
{
    [UsedImplicitly]
    public class MarkAnswerAsCorrectCommandHandler : ICommandHandler<MarkAnswerAsCorrectCommand>
    {
        private readonly IRepository<Question> _questionRepository;

        private readonly ICommandHandler<Core.Command.MarkAnswerAsCorrectCommand> _commandHandler;

        public MarkAnswerAsCorrectCommandHandler(IRepository<Question> questionRepository,
            ICommandHandler<Core.Command.MarkAnswerAsCorrectCommand> commandHandler)
        {
            _questionRepository = questionRepository;
            _commandHandler = commandHandler;
        }

        public async Task ExecuteAsync(MarkAnswerAsCorrectCommand message, CancellationToken token)
        {
            var question = await _questionRepository.LoadAsync(message.QuestionId, token);

            await _commandHandler.ExecuteAsync(new Core.Command.MarkAnswerAsCorrectCommand(message.AnswerId,
                question.User.Id),token);
        }
    }
}