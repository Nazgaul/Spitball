using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public class ChatTextMessage : ChatMessage
    {
        protected ChatTextMessage()
        {

        }

        public ChatTextMessage(RegularUser user, string message, ChatRoom room) : base(user, room)
        {
            Message = message;

        }

        public virtual string Message { get; protected set; }

    }
}