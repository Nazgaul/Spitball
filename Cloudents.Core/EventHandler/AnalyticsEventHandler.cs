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
            return _googleAnalytics.TrackEventAsync("Tutor funnel", "Tutor approved", eventMessage.TutorId.ToString(), token);
        }

        public Task HandleAsync(StudyRoomSessionCreatedEvent eventMessage, CancellationToken token)
        {
            if (eventMessage.StudyRoomSession.StudyRoom.Tutor.AdminUser != null)
            {
                return Task.CompletedTask;
            }

            var tutorId = eventMessage.StudyRoomSession.StudyRoom.Tutor.Id;
            var identifier = eventMessage.StudyRoomSession.StudyRoom.Id;
            return _googleAnalytics.TrackEventAsync("Tutor funnel", "Tutor session", $"{tutorId}-{identifier}", token);
        }
    }
}