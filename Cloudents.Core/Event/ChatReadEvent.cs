using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class ChatReadEvent : IEvent
    {
        public ChatReadEvent(ChatUser chatUser)
        {
            ChatUser = chatUser;
        }

        public ChatUser ChatUser { get; }
    }
}