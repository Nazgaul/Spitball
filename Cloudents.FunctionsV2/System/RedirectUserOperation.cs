﻿using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.System
{
    public class RedirectUserOperation : ISystemOperation<RedirectUserMessage>
    {
        private readonly ICommandBus _commandBus;

        public RedirectUserOperation(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }


        public async Task DoOperationAsync(RedirectUserMessage msg, IBinder binder, CancellationToken token)
        {
            var command = new CreateUrlStatsCommand(msg.Host, DateTime.UtcNow, msg.Url.ToString(), msg.Referer, msg.Location, msg.UserIp);
            await _commandBus.DispatchAsync(command, token);
        }
    }
}