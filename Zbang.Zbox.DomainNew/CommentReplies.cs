using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Throw.OnNull(user, "User");
            Throw.OnNull(box, "box");
            Throw.OnNull(question, "question");
            Throw.OnNull(text, "text", false);
            Id = id;

            User = user;
            Box = box;
            Text = text.Trim();
            DateTimeUser = new UserTimeDetails(user.Email);
            Question = question;
            MarkAnswer = false;
            Items = items;
            Box.UserTime.UpdateUserTime(user.Email);
        }

        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual string Text { get; set; }
        public virtual Box Box { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual UserTimeDetails DateTimeUser { get; set; }

        public virtual Comment Question { get; set; }
        public virtual bool MarkAnswer { get; set; }

        public virtual int RatingCount { get; internal set; }


        public void RemoveItem(Item item)
        {
            Items.Remove(item);
        }  
    }
}
