using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class ChangeCountryEvent : IEvent
    {
        public ChangeCountryEvent(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; private set; }
    }
}
