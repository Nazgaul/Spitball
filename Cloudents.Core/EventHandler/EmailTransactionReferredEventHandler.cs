using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class EmailTransactionReferredEventHandler : IEventHandler<TransactionReferredEvent>
    {

        private readonly IQueueProvider _serviceBusProvider;


        public EmailTransactionReferredEventHandler(IQueueProvider serviceBusProvider)
        {
            _serviceBusProvider = serviceBusProvider;
        }

        public async Task HandleAsync(TransactionReferredEvent eventMessage, CancellationToken token)
        {
            await _serviceBusProvider.InsertMessageAsync(
                  new ReferralBonusEmail(eventMessage.Transaction.User.Email, eventMessage.Transaction.User.Culture), token);
        }
    }
}