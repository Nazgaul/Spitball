using System;

namespace Zbang.Zbox.Domain
{
    public class CommentLike
    {
        protected CommentLike()
        {

        }

        public CommentLike(Comment comment, User user, Guid id)
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = id;
            User = user;
            Comment = comment;
            CreationTime = DateTime.UtcNow;
            Box = comment.Box;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public virtual Guid Id { get; protected set; }

        public virtual User User { get; protected set; }

        public virtual Comment Comment { get; protected set; }
        public virtual DateTime CreationTime { get; protected set; }

        public virtual Box Box { get; protected set; }
    }
}
