using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Item.Commands.FlagItem;
using Cloudents.Domain.Entities;

namespace Cloudents.Core.EventHandler
{
    public class FlagAnItemEventHandler : IEventHandler<ItemFlaggedEvent>
    {
        private readonly ICommandBus _commandBus;

        public FlagAnItemEventHandler(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public async Task HandleAsync(ItemFlaggedEvent eventMessage, CancellationToken token)
        {
            //TODO: this is very very ugly
            if (eventMessage.Obj is Document p)
            {
                var command = new FlagDocumentCommand(p.Id);
                await _commandBus.DispatchAsync(command, token);
            }
            if (eventMessage.Obj is Answer p2)
            {
                var command = new FlagAnswerCommand(p2.Id);
                await _commandBus.DispatchAsync(command, token);
            }
            if (eventMessage.Obj is Question p3)
            {
                var command = new FlagQuestionCommand(p3.Id);
                await _commandBus.DispatchAsync(command, token);
            }
        }
    }
}