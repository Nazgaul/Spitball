using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.EventHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UpdateUserBalanceEventHandler : IEventHandler<QuestionDeletedAdminEvent>, IEventHandler<AnswerDeletedAdminEvent>
    {
        private readonly ICommandBus _commandBus;

        public UpdateUserBalanceEventHandler(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public async Task HandleAsync(QuestionDeletedAdminEvent eventMessage, CancellationToken token)
        {
            var command = new UpdateUserBalanceCommand(eventMessage.UserIds);
            await _commandBus.DispatchAsync(command, token);
        }

        public async Task HandleAsync(AnswerDeletedAdminEvent answerEventMessage, CancellationToken token)
        {
            var command = new UpdateUserBalanceCommand(answerEventMessage.UserIds);
            await _commandBus.DispatchAsync(command, token);
        }
    }
}
