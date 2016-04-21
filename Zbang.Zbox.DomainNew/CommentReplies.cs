using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class CommentReplies //: ISoftDeletable
    {
        protected CommentReplies()
        {

        }

        public CommentReplies(User user, string text, Box box, Guid id, Comment question, IList<Item> items)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (box == null)
            {
                throw new ArgumentNullException(nameof(box));
            }
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = id;

            User = user;
            Box = box;
            if (string.IsNullOrEmpty(text))
            {
                text = null;

            }
            Text = text?.Trim();
            DateTimeUser = new UserTimeDetails(user.Id);
            Question = question;
            Items = items;
            Box.UserTime.UpdateUserTime(user.Id);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

        }

        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual string Text { get; protected set; }
        public virtual Box Box { get; set; }
        protected virtual ICollection<Item> Items { get; set; }
        protected virtual UserTimeDetails DateTimeUser { get; set; }

        public virtual Comment Question { get; set; }

        protected virtual ICollection<Updates> Updates { get; set; }


        protected virtual ICollection<ReplyLike> Likes { get; set; }
        public virtual int LikeCount { get; set; }


        //public bool IsDeleted
        //{
        //    get;
        //    set;
        //}

        //public void DeleteAssociation()
        //{
        //    Updates.Clear();
        //}
    }
}
