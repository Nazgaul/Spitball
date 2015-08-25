using System;

namespace Zbang.Zbox.Domain
{
    public class Updates
    {
        // ReSharper disable DoNotCallOverridableMethodsInConstructor

        protected Updates()
        {

        }

        private Updates(User user, Box box)
        {
            User = user;
            Box = box;
            CreationTime = DateTime.UtcNow;
        }
       
        public Updates(User user, Box box, Quiz quiz)
            : this(user, box)
        {
            Quiz = quiz;
        }
        public Updates(User user, Box box, Item item)
            : this(user, box)
        {
            Item = item;
        }

        public Updates(User user, Box box, Comment comment) : this(user,box)
        {
            Comment = comment;
        }

        public Updates(User user, Box box, CommentReplies reply) : this(user,box)
        {
            Reply = reply;
            Comment = reply.Question;
        }

        // ReSharper restore DoNotCallOverridableMethodsInConstructor
       
        public virtual Guid Id { get; set; }
        public virtual User User { get; protected set; }
        public virtual Box Box { get; protected set; }
        public virtual Comment Comment { get; protected set; }
        public virtual CommentReplies Reply { get; protected set; }
        public virtual Item Item { get; protected set; }
        public virtual Quiz Quiz { get; protected set; }

        public virtual DateTime CreationTime { get; protected set; }
    }
}
