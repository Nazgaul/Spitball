using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public class ChatTextMessage : ChatMessage
    {
        protected ChatTextMessage()
        {

        }

        public ChatTextMessage(User user, string message, ChatRoom room) : base(user, room)
        {
            Message = message;

        }

        public virtual string Message { get; protected set; }

    }

    //public class SystemTextMessage : ChatTextMessage
    //{
    //    protected SystemTextMessage()
    //    {

    //    }

    //    public SystemTextMessage(User user, string message, ChatRoom room) : base(user, message, room)
    //    {
    //    }
    //}
}