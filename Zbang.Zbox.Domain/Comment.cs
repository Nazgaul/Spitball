using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class Comment : ICommentTarget
    {
        protected Comment()
        {
        }

        public Comment(User author, string commentText)
        {
            this.CommentText = commentText;
                    
            this.Author = author;
            this.Replies = new HashSet<Comment>();

            DateTime now = DateTime.UtcNow;
            CreationTimeUtc = now;
            CreationTimeEpochMillis = Convert.ToInt64((now - ModelConstants.StartOfEpoch).TotalMilliseconds);
        }

        //persistent properties
        public virtual int Id { get; protected set; }

        public virtual string CommentText { get; internal protected set; }

        public virtual User Author { get; protected set; }

        public virtual DateTime CreationTimeUtc { get; set; }

        public virtual long CreationTimeEpochMillis { get; set; }

        internal protected virtual ICollection<Comment> Replies { get; protected set; }

        public ICollection<Comment> Comments
        {
            get { return Replies; }
        }

        public Comment AddComment(User author, string commentText)
        {
            Comment comment = new Comment(author, commentText);
            this.Replies.Add(comment);
            return comment;
        }
    }
}
