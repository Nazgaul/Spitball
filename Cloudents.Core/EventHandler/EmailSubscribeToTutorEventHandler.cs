using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    public class EmailSubscribeToTutorEventHandler : EmailEventHandler, IEventHandler<SubscribeToTutorEvent>
    {
        public EmailSubscribeToTutorEventHandler(IQueueProvider serviceBusProvider) : base(serviceBusProvider)
        {
        }

        public async Task HandleAsync(SubscribeToTutorEvent eventMessage, CancellationToken token)
        {

            var follow = eventMessage.Follow;
            var tutor = follow.User;
            var subscriptionPrice = tutor.Tutor?.SubscriptionPrice ?? throw new NullReferenceException();

            var user = follow.User;
            var email = new SubscribeTutorMessage(tutor.Email, tutor.FirstName, user.FirstName, subscriptionPrice, user.Email);
            await SendEmailAsync(email, token);
        }
    }
}