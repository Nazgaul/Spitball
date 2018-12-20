using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

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