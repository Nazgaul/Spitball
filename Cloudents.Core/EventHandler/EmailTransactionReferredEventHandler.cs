using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Event;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Message.Email;
using Cloudents.Application.Storage;
using Cloudents.Common.Enum;

namespace Cloudents.Application.EventHandler
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class EmailTransactionReferredEventHandler : EmailEventHandler, IEventHandler<TransactionEvent>
    {


        public EmailTransactionReferredEventHandler(IQueueProvider serviceBusProvider): base(serviceBusProvider)
        {
        }

        public  Task HandleAsync(TransactionEvent eventMessage, CancellationToken token)
        {
            if (eventMessage.Transaction.Action == TransactionActionType.ReferringUser)
            {
                return SendEmail(
                    new ReferralBonusEmail(eventMessage.Transaction.User.Email, eventMessage.Transaction.User.Culture)
                    , eventMessage.Transaction.User, token);
            }

            return Task.CompletedTask;
        }
    }
}