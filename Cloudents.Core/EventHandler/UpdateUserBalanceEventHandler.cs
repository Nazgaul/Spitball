using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UpdateUserBalanceEventHandler : IEventHandler<TransactionEvent>
    {
        private readonly IQueueProvider _queueProvider;

        public UpdateUserBalanceEventHandler(IQueueProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }


        public Task HandleAsync(TransactionEvent transactionEventMessage, CancellationToken token)
        {
            return _queueProvider.InsertMessageAsync(new UpdateUserBalanceMessage(new List<long>()
            {
                transactionEventMessage.Transaction.User.Id
            }), token);
        }
    }
}
