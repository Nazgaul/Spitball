﻿using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;

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