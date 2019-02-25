//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Command;
//using Cloudents.Command.Command;
//using Cloudents.Core.Message.System;
//using Microsoft.Azure.WebJobs;

//namespace Cloudents.FunctionsV2.System
//{
//    public class UpdateUserTagOperation : ISystemOperation<AddUserTagMessage>
//    {
//        private readonly ICommandBus _commandBus;

//        public UpdateUserTagOperation(ICommandBus commandBus)
//        {
//            _commandBus = commandBus;
//        }

//        public async Task DoOperationAsync(AddUserTagMessage msg, IBinder binder, CancellationToken token)
//        {
//            var command = new AddUserTagCommand(msg.UserId, msg.Tag);
//            await _commandBus.DispatchAsync(command, token);
//        }
//    }
//}