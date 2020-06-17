using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class UpdateTutorSettingsEvent : IEvent
    {
        public UpdateTutorSettingsEvent(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; private set; }
    }

    public class UserChangeNameEvent : IEvent
    {
        public UserChangeNameEvent(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; private set; }
    }

    public class TutorSubscriptionEvent : IEvent
    {
        public TutorSubscriptionEvent(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; private set; }
    }
}
