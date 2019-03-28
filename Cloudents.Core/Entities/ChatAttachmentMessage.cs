using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public class ChatAttachmentMessage : ChatMessage
    {
        protected ChatAttachmentMessage()
        {

        }

        public ChatAttachmentMessage(RegularUser user, string blob, ChatRoom room) : base(user, room)
        {
            Blob = blob;

        }

        public virtual string Blob { get; protected set; }

    }
}