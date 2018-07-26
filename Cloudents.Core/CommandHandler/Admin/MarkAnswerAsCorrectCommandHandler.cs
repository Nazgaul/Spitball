using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Attributes;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler.Admin
{
    [AdminCommandHandler]
    public class MarkAnswerAsCorrectCommandHandler : ICommandHandler<Command.Admin.MarkAnswerAsCorrectCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly ICommandHandler<MarkAnswerAsCorrectCommand> _commandHandler;

        public MarkAnswerAsCorrectCommandHandler(IRepository<Question> questionRepository,
            ICommandHandler<MarkAnswerAsCorrectCommand> commandHandler)
        {
            _questionRepository = questionRepository;
            _commandHandler = commandHandler;
        }


        public async Task ExecuteAsync(Command.Admin.MarkAnswerAsCorrectCommand message, CancellationToken token)
        {
            var question = await _questionRepository.LoadAsync(message.QuestionId, token);
            await _commandHandler.ExecuteAsync(new MarkAnswerAsCorrectCommand(message.AnswerId,
                question.User.Id), token);
        }
    }
}