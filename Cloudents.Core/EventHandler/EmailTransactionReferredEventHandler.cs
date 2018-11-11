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
    public class EmailTransactionReferredEventHandler : EmailEventHandler, IEventHandler<TransactionReferredEvent>
    {


        public EmailTransactionReferredEventHandler(IQueueProvider serviceBusProvider): base(serviceBusProvider)
        {
        }

        public async Task HandleAsync(TransactionReferredEvent eventMessage, CancellationToken token)
        {
            await SendEmail(
                  new ReferralBonusEmail(eventMessage.Transaction.User.Email, eventMessage.Transaction.User.Culture)
                  , eventMessage.Transaction.User, token);
        }
    }
}