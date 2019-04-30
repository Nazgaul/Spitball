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
            //eventMessage.StudyRoom.Users.Where(w=>w.)
            //StudentPaymentMessage
            throw new System.NotImplementedException();
        }
    }
}