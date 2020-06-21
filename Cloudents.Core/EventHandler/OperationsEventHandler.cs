using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    public class OperationsEventHandler :
        IEventHandler<LeadEvent>
      //  IEventHandler<AddUserBroadcastStudyRoomEvent>
    {
        private readonly IQueueProvider _queueProvider;

        public OperationsEventHandler(IQueueProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }

        public Task HandleAsync(LeadEvent eventMessage, CancellationToken token)
        {
            var message = new RequestTutorMessage(eventMessage.Lead.Id);
            return _queueProvider.InsertMessageAsync(message, token);
        }

        //public async Task HandleAsync(AddUserBroadcastStudyRoomEvent eventMessage, CancellationToken token)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}