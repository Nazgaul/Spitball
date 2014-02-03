using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;

namespace Zbang.Zbox.Domain
{
    public class Question
    {
        protected Question()
        {

        }
        public Question(User user, string text, Box box, Guid id, IList<Item> items)
        {
            Throw.OnNull(user, "User");
            Throw.OnNull(box, "box");
            Throw.OnNull(text, "text", false);
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
        protected virtual ICollection<Item> Items { get; set; }
        protected virtual ICollection<Answer> Answers { get; set; }

        public ICollection<Answer> AnswersReadOnly { get { return Answers.ToList().AsReadOnly(); } }

        
        public virtual UserTimeDetails DateTimeUser { get; set; }

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
        }    
    }
}
