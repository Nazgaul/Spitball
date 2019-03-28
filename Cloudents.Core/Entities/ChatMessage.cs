using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public abstract class ChatMessage : Entity<Guid>
    {
        protected ChatMessage()
        {

        }

       

        protected ChatMessage(RegularUser user, ChatRoom room)
        {
            User = user;
            CreationTime = DateTime.UtcNow;
            ChatRoom = room;
        }



        // public virtual Guid Id { get; protected set; }

        public virtual RegularUser User { get; protected set; }
        public virtual DateTime CreationTime { get; protected set; }

        public virtual ChatRoom ChatRoom { get; protected set; }

    }
}