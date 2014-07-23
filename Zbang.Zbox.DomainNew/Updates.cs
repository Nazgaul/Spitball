using System;

namespace Zbang.Zbox.Domain
{
    public class Updates
    {
        protected Updates()
        {

        }
        public Updates(User user, Box box,
            Comment comment = null, CommentReplies reply = null, Item item = null,
            Quiz quiz = null
            )
        {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            User = user;
            Box = box;
            Comment = comment;
            Reply = reply;
            Item = item;
            Quiz = quiz;
            CreationTime = DateTime.UtcNow;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

        }
       
        public virtual Guid Id { get; set; }
        public virtual User User { get; protected set; }
        public virtual Box Box { get; protected set; }
        public virtual Comment Comment { get; protected set; }
        public virtual CommentReplies Reply { get; protected set; }
        public virtual Item Item { get; protected set; }
        public virtual Quiz Quiz { get; protected set; }

        protected virtual DateTime CreationTime { get; set; }
    }
}
