using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class ChatMessageEvent : IEvent
    {
        public ChatMessageEvent(ChatMessage chatMessage)
        {
            ChatMessage = chatMessage;
        }

        public ChatMessage ChatMessage { get; }
    }

    //public class OfflineChatMessageEvent : IEvent
    //{
    //    public OfflineChatMessageEvent(ChatUser chatUser)
    //    {
    //        ChatUser = chatUser;
    //    }

    //    public ChatUser ChatUser { get; }
    //}
}