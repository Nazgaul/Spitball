using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    public class EmailPaymentDetailsEventHandler : IEventHandler<StudyRoomCreatedEvent>
    {

        private readonly IQueueProvider _serviceBusProvider;

        public EmailPaymentDetailsEventHandler(IQueueProvider serviceBusProvider)
        {
            _serviceBusProvider = serviceBusProvider;
        }

        public async Task HandleAsync(StudyRoomCreatedEvent eventMessage, CancellationToken token)
        {
            //var users = eventMessage.StudyRoom.Users.Where(w => w.User.Id != w.Room.Tutor.Id)
            //    .Select(s => s.User).Where(w => w.BuyerPayment?.IsValid() ?? false);

            //foreach (var user in users)
            //{
            await _serviceBusProvider.InsertMessageAsync(new StudentPaymentMessage(eventMessage.StudyRoom.Id), token);


            //}
            //StudentPaymentMessage
            //throw new System.NotImplementedException();
        }
    }
}