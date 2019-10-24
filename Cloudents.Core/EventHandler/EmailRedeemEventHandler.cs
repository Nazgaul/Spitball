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
        private readonly IQueueProvider _serviceBusProvider;


        public EmailRedeemEventHandler(IQueueProvider serviceBusProvider)
        {
            _serviceBusProvider = serviceBusProvider;
        }

        public async Task HandleAsync(TransactionEvent redeemEventMessage, CancellationToken token)
        {
            if (redeemEventMessage.Transaction.Action == TransactionActionType.CashOut)
            {
                await _serviceBusProvider.InsertMessageAsync(
                    new SupportRedeemEmail(redeemEventMessage.Transaction.Price,
                        redeemEventMessage.User.Id), token);
            }

            if (redeemEventMessage.Transaction.Action == TransactionActionType.SoldDocument)
            {
                var message = new DocumentPurchasedMessage(redeemEventMessage.Transaction.Id);

                await _serviceBusProvider.InsertMessageAsync(message, token);
            }
        }
    }
}
