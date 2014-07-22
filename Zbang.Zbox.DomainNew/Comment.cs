using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Domain
{
    public class Comment
    {
        protected Comment()
        {

        }
        public Comment(User user, string text, Box box, Guid id, IList<Item> items)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (box == null)
            {
                throw new ArgumentNullException("box");
            }
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            Id = id;
            Items = items;
            User = user;
            Box = box;
            Text = text.Trim();
            DateTimeUser = new UserTimeDetails(user.Email);
            Box.UserTime.UpdateUserTime(user.Email);
        }
        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual string Text { get; set; }
        public virtual Box Box { get; set; }
        protected virtual ICollection<Item> Items { get; private set; }
        protected virtual ICollection<CommentReplies> Answers { get; private set; }

        public ICollection<CommentReplies> AnswersReadOnly { get { return Answers.ToList().AsReadOnly(); } }


        public virtual UserTimeDetails DateTimeUser { get; set; }

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
        }
    }
}
