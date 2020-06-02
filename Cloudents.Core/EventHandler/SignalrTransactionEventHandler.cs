using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class SignalrTransactionEventHandler :
            IEventHandler<TransactionEvent>
    {
        private readonly IServiceBusProvider _queueProvider;

        public SignalrTransactionEventHandler(IServiceBusProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }


       

        public Task HandleAsync(TransactionEvent eventMessage, CancellationToken token)
        {
            var message = new SignalRTransportType(SignalRType.User,
                SignalRAction.Update, new { balance = eventMessage.User.Transactions.Balance });

            return _queueProvider.InsertMessageAsync
                (message, eventMessage.User.Id, token);
        }

      
    }
}
