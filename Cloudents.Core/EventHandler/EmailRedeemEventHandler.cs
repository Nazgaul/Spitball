using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class EmailRedeemEventHandler : IEventHandler<TransactionEvent>
    {
        private readonly IQueueProvider _queueProvider;


        public EmailRedeemEventHandler(IQueueProvider serviceBusProvider)
        {
            _queueProvider = serviceBusProvider;
        }

        public async Task HandleAsync(TransactionEvent redeemEventMessage, CancellationToken token)
        {
            if (redeemEventMessage.Transaction.Action == TransactionActionType.CashOut)
            {
                await _queueProvider.InsertMessageAsync(
                    new RedeemTransactionMessage(redeemEventMessage.Transaction.Id), token);
            }

            if (redeemEventMessage.Transaction.Action == TransactionActionType.SoldDocument)
            {
                var message = new DocumentPurchasedMessage(redeemEventMessage.Transaction.Id);

                await _queueProvider.InsertMessageAsync(message, token);
            }
        }
    }

    public class LeadEventHandler : IEventHandler<LeadEvent>
    {
        private readonly IQueueProvider _queueProvider;

        public LeadEventHandler(IQueueProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }

        public async Task HandleAsync(LeadEvent eventMessage, CancellationToken token)
        {
            var message = new RequestTutorMessage(eventMessage.Lead.Id);
            await _queueProvider.InsertMessageAsync(message, token);
        }
    }
}
