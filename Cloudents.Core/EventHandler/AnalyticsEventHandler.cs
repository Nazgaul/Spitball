using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.EventHandler
{
    public class AnalyticsEventHandler : IEventHandler<TutorApprovedEvent>,
        IEventHandler<StudyRoomSessionCreatedEvent>
    {
        private readonly IGoogleAnalytics _googleAnalytics;

        public AnalyticsEventHandler(IGoogleAnalytics googleAnalytics)
        {
            _googleAnalytics = googleAnalytics;
        }

        public Task HandleAsync(TutorApprovedEvent eventMessage, CancellationToken token)
        {
            return _googleAnalytics.TrackEventAsync("Tutor funnel", "Tutor approved", eventMessage.UserId.ToString(), token);
        }

        public async Task HandleAsync(StudyRoomSessionCreatedEvent eventMessage, CancellationToken token)
        {
           //eventMessage.StudyRoomSession.StudyRoom.Identifier
        }
    }
}