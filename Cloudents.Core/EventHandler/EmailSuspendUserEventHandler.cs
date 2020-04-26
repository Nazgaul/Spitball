using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.EventHandler
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class EmailSuspendUserEventHandler : EmailEventHandler, IEventHandler<UserSuspendEvent>
    {



        public EmailSuspendUserEventHandler(IQueueProvider serviceBusProvider) : base(serviceBusProvider)
        {
        }

        public async Task HandleAsync(UserSuspendEvent eventMessage, CancellationToken token)
        {
            await SendEmail(
                  new SuspendUserEmail(eventMessage.User.Email, eventMessage.User.Language), token);
        }
    }


    public class EmailWelcomeTutorEventHandler : EmailEventHandler, IEventHandler<TutorCreatedEvent>
    {
        public EmailWelcomeTutorEventHandler(IQueueProvider serviceBusProvider) : base(serviceBusProvider)
        {
        }


        

        public async Task HandleAsync(TutorCreatedEvent eventMessage, CancellationToken token)
        {
            if (eventMessage.Tutor.User.SbCountry == Country.UnitedStates)
            {
                var email = new WelcomeTutorEmail(eventMessage.Tutor.User.Email, Language.English,
                    eventMessage.Tutor.User.FirstName);
                await SendEmail(email, token);
            }
        }
    }
}
