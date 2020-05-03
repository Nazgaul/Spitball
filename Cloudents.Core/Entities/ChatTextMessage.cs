using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public class ChatTextMessage : ChatMessage
    {
        [SuppressMessage("ReSharper", "CS8618", Justification = "nhibernate")]
        protected ChatTextMessage()
        {

        }

        public ChatTextMessage(User user, string message, ChatRoom room) : base(user, room)
        {
            Message = message;

        }

        public virtual string Message { get; protected set; }

    }
}