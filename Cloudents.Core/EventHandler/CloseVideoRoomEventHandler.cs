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
        public async Task HandleAsync(EndStudyRoomSessionEvent eventMessage, CancellationToken token)
        {
            await _videoProvider.CloseRoomAsync(eventMessage.Session.SessionId);
        }
    }
}
