using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class EmailRedeemEventHandler: IEventHandler<RedeemEvent>
    {
        private readonly IQueueProvider _serviceBusProvider;

        public EmailRedeemEventHandler(IQueueProvider serviceBusProvider)
        {
            _serviceBusProvider = serviceBusProvider;
        }

        public async Task HandleAsync(RedeemEvent redeemEventMessage, CancellationToken token)
        {
            await _serviceBusProvider.InsertMessageAsync(new SupportRedeemEmail(redeemEventMessage.Amount, redeemEventMessage.UserId), token);
        }
    }
}
