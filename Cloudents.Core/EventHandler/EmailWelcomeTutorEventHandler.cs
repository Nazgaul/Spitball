//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Event;
//using Cloudents.Core.Interfaces;
//using Cloudents.Core.Message.Email;
//using Cloudents.Core.Storage;

//namespace Cloudents.Core.EventHandler
//{
//    public class EmailWelcomeTutorEventHandler : EmailEventHandler, IEventHandler<TutorCreatedEvent>
//    {
//        public EmailWelcomeTutorEventHandler(IQueueProvider serviceBusProvider) : base(serviceBusProvider)
//        {
//        }


        

//        public async Task HandleAsync(TutorCreatedEvent eventMessage, CancellationToken token)
//        {
//            if (eventMessage.Tutor.User.SbCountry == Country.UnitedStates)
//            {
//                var email = new WelcomeTutorEmail(eventMessage.Tutor.User.Email, Language.English,
//                    eventMessage.Tutor.User.FirstName);
//                await SendEmailAsync(email, token);
//            }
//        }
//    }
//}