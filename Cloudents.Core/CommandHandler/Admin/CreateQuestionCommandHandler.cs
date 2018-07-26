using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler.Admin
{
    public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand>
    {

        private readonly ICommandHandler<Core.Command.CreateQuestionCommand> _commandHandler;
        private readonly IUserRepository _userRepository;

        public CreateQuestionCommandHandler(ICommandHandler<Command.CreateQuestionCommand> commandHandler, IUserRepository userRepository)
        {
            _commandHandler = commandHandler;
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(CreateQuestionCommand message, CancellationToken token)
        {
            var user = await _userRepository.GetRandomFictiveUserAsync(token);
            var command = new Command.CreateQuestionCommand
            {
                Text = message.Text,
                Price = message.Price,
                Files = message.Files,
                SubjectId = message.SubjectId,
                UserId = user.Id
            };
            await _commandHandler.ExecuteAsync(command, token);
        }
    }
}