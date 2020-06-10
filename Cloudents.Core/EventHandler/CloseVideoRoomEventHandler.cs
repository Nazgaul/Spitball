using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class CloseVideoRoomEventHandler : IEventHandler<EndStudyRoomSessionEvent>
    {
        private readonly IStudyRoomProvider _videoProvider;
        public CloseVideoRoomEventHandler(IStudyRoomProvider videoProvider)
        {
            _videoProvider = videoProvider;
        }
        public Task HandleAsync(EndStudyRoomSessionEvent eventMessage, CancellationToken token)
        {
            return _videoProvider.CloseRoomAsync(eventMessage.Session.SessionId);
        }
    }
}
