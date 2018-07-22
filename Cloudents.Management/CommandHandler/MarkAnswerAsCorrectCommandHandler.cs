using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Cloudents.Management.Command;
using JetBrains.Annotations;

namespace Cloudents.Management.CommandHandler
{
    [UsedImplicitly]
    public class MarkAnswerAsCorrectCommandHandler : ICommandHandler<MarkAnswerAsCorrectCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Answer> _answerRepository;
        private readonly IServiceBusProvider _serviceBusProvider;

        private readonly ICommandHandler<Cloudents.Core.Command.MarkAnswerAsCorrectCommand> _commandHandler;

        public MarkAnswerAsCorrectCommandHandler(IRepository<Question> questionRepository,
            IRepository<Answer> answerRepository, IServiceBusProvider serviceBusProvider, ICommandHandler<Core.Command.MarkAnswerAsCorrectCommand> commandHandler)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _serviceBusProvider = serviceBusProvider;
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