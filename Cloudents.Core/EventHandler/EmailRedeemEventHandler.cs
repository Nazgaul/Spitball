using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class EmailRedeemEventHandler : IEventHandler<TransactionEvent>
    {
        private readonly IQueueProvider _serviceBusProvider;
        private readonly ISystemEventRepository _eventRepository;


        public EmailRedeemEventHandler(IQueueProvider serviceBusProvider, ISystemEventRepository eventRepository)
        {
            _serviceBusProvider = serviceBusProvider;
            _eventRepository = eventRepository;
        }

        public async Task HandleAsync(TransactionEvent redeemEventMessage, CancellationToken token)
        {
            if (redeemEventMessage.Transaction.TransactionType.Action == TransactionActionType.CashOut)
            {
                await _serviceBusProvider.InsertMessageAsync(
                    new SupportRedeemEmail(redeemEventMessage.Transaction.TransactionType.Price, redeemEventMessage.User.Id), token);
            }

            if (redeemEventMessage.Transaction.TransactionType.Action == TransactionActionType.SoldDocument)
            {
                var email = await _eventRepository.GetEmailAsync(SystemEvent.DocumentPurchased, redeemEventMessage.User.Language, token);

            }
        }
    }
}
