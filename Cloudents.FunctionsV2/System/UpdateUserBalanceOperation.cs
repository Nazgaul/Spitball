using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Command.Admin;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Message.System;
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
            var command = new UpdateUserBalanceCommand(msg.UsersIds);
            await _commandBus.DispatchAsync(command, token);
        }
    }
}