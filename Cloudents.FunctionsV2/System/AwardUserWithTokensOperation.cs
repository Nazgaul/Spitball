//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Command;
//using Cloudents.Command.Command.Admin;
//using Cloudents.Core.Message.System;
//using Microsoft.Azure.WebJobs;

//namespace Cloudents.FunctionsV2.System
//{
//    public class AwardUserWithTokensOperation : ISystemOperation<AwardUserWithTokens>
//    {
//        private readonly ICommandBus _commandBus;

//        public AwardUserWithTokensOperation(ICommandBus commandBus)
//        {
//            _commandBus = commandBus;
//        }

//        public async Task DoOperationAsync(AwardUserWithTokens msg, IBinder binder, CancellationToken token)
//        {
//            var command = new DistributeTokensCommand(msg.Id, msg.Amount, msg.Type);
//            await _commandBus.DispatchAsync(command, token);
//        }
//    }
//}