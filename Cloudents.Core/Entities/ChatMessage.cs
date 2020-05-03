using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public abstract class ChatMessage : Entity<Guid>
    {
        [SuppressMessage("ReSharper", "CS8618", Justification = "nhibernate")]
        protected ChatMessage()
        {

        }



        protected ChatMessage(User user, ChatRoom room)
        {
            User = user;
            CreationTime = DateTime.UtcNow;
            ChatRoom = room;
        }



        // public virtual Guid Id { get; protected set; }

        public virtual User User { get; protected set; }
        public virtual DateTime CreationTime { get; protected set; }

        public virtual ChatRoom ChatRoom { get; protected set; }

    }
}