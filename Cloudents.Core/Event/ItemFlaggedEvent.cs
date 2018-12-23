using Cloudents.Core.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Core.Event
{
    public class ItemFlaggedEvent : IEvent
    {
        public ItemObject Obj { get; }

        public ItemFlaggedEvent(ItemObject obj)
        {
            Obj = obj;
        }
    }
}