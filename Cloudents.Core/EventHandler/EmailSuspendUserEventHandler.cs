using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
                var countriesNotToSend = new[]
                {
                    "DZ", "AO", "BJ", "BW", "BF", "BI", "CM",
                    "CV", "CF", "KM", "CD", "DJ", "EG", "GQ", "ER", "ET", "GA", "GM", "GH", "GN", "GW", "CI", "KE",
                    "LS", "LR", "LY",
                    "MG", "MW", "ML", "MR", "MU", "MA", "MZ", "NA", "NE", "NG", "CG", "RE", "RW", "SH", "ST", "SN",
                    "SC", "SL", "SO",
                    "SS", "SD", "SZ", "TZ", "TG", "TN", "UG", "EH", "ZM", "ZW", "AF", "AM", "AZ", "BH", "BD", "BT",
                    "BN", "KH", "GE",
                    "IN", "ID", "IR", "IQ", "JO", "KZ", "KW", "KG", "LA", "LB", "MO", "MY", "MV", "MN", "MM", "NP",
                    "KP", "OM", "PK",
                    "PH", "QA", "SA", "LK", "SY", "TW", "TJ", "TH", "TR", "TM", "AE", "UZ", "VN", "YE"
                };
                if (countriesNotToSend.Contains(eventMessage.Tutor.User.Country))
                {
                    return;
                }

                var email = new WelcomeTutorEmail(eventMessage.Tutor.User.Email, Language.English,
                    eventMessage.Tutor.User.FirstName);
                await SendEmail(email, token);
            }
        }
    }
}
