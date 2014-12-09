using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Domain
{
    public class CommentReplies
    {
        protected CommentReplies()
        {

        }

        public CommentReplies(User user, string text, Box box, Guid id, Comment question, IList<Item> items)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (box == null)
            {
                throw new ArgumentNullException("box");
            }
            if (question == null)
            {
                throw new ArgumentNullException("question");
            }
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = id;

            User = user;
            Box = box;
            if (text != null)
            {
                text = text.Trim();
            }
            Text = text;
            DateTimeUser = new UserTimeDetails(user.Email);
            Question = question;
            Items = items;
            Box.UserTime.UpdateUserTime(user.Email);
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

        
    }
}
