using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message.Email;

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
}
