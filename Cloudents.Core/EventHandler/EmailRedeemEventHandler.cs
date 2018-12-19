using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Event;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Message;
using Cloudents.Application.Storage;

namespace Cloudents.Application.EventHandler
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
