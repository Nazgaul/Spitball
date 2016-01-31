using System;

namespace Zbang.Zbox.Domain
{
    public class ReplyLike
    {
        protected ReplyLike()
        {

        }

        public ReplyLike(CommentReplies reply, User user, Guid id)
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = id;
            User = user;
            Reply = reply;
            CreationTime = DateTime.UtcNow;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public virtual Guid Id { get; protected set; }

        public virtual User User { get; protected set; }

        public virtual CommentReplies Reply { get; protected set; }
        public virtual DateTime CreationTime { get; protected set; }
    }
}
