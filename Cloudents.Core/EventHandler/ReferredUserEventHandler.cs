using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    class ReferredUserEventHandler : IEventHandler<ReferredUserEvent>
    {
       
        private readonly IQueueProvider _serviceBusProvider;
       

        public ReferredUserEventHandler(IQueueProvider serviceBusProvider)
        {
            _serviceBusProvider = serviceBusProvider;
        }

        public async Task HandleAsync(ReferredUserEvent eventMessage, CancellationToken token)
        {
            await _serviceBusProvider.InsertMessageAsync(
                  new ReferralBonusEmail(eventMessage.Transaction.User.Email, eventMessage.Transaction.User.Culture), token);
        }
    }
}