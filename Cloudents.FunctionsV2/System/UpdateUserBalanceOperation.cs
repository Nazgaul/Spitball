using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.System
{
    public class UpdateUserBalanceOperation : ISystemOperation
    {
        private readonly ICommandBus _commandBus;

        public UpdateUserBalanceOperation(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public async Task DoOperationAsync(BaseSystemMessage msg, IBinder binder, CancellationToken token)
        {
            var command = new UpdateUserBalanceCommand(msg.GetData());
            await _commandBus.DispatchAsync(command, token);
        }
    }
}