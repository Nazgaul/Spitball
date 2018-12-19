using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Event
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