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

        public  Task HandleAsync(StudyRoomCreatedEvent eventMessage, CancellationToken token)
        {
            return Task.CompletedTask;
         //   await _serviceBusProvider.InsertMessageAsync(new StudentPaymentMessage(eventMessage.StudyRoom.Id), token);
        }
    }
}