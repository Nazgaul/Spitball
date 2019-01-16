using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class EmailTransactionReferredEventHandler : EmailEventHandler, IEventHandler<TransactionEvent>
    {


        public EmailTransactionReferredEventHandler(IQueueProvider serviceBusProvider): base(serviceBusProvider)
        {
        }

        public  Task HandleAsync(TransactionEvent eventMessage, CancellationToken token)
        {
            if (eventMessage.Transaction.TransactionType.Action == TransactionActionType.ReferringUser)
            {
                return SendEmail(
                    new ReferralBonusEmail(eventMessage.Transaction.User.Email, eventMessage.Transaction.User.Language)
                    , eventMessage.Transaction.User, token);
            }

            return Task.CompletedTask;
        }
    }
}