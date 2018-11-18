using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.System
{
    public class UpdateUserBalanceOperation : ISystemOperation<UpdateUserBalanceMessage>
    {
        private readonly ICommandBus _commandBus;

        public UpdateUserBalanceOperation(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public async Task DoOperationAsync(UpdateUserBalanceMessage msg, IBinder binder, CancellationToken token)
        {
            var command = new UpdateUserBalanceCommand();
            await _commandBus.DispatchAsync(command, token);
        }
    }
}