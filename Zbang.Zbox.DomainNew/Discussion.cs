using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
    public class Discussion
    {
        protected Discussion()
        {

        }
        public Discussion(Guid id, User owner, string text, Question question)
        {
            Id = id;
            Owner = owner;
            Text = text.Trim();
            CreationTime = DateTime.UtcNow;
            Quiz = question.Quiz;
            Question = question;
            Quiz.Box.UserTime.UpdateUserTime(owner.Email);
        }
        public virtual Guid Id { get; set; }
        public virtual User Owner { get; set; }
        public virtual string Text { get; set; }
        public virtual DateTime CreationTime { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual Question Question { get; set; }
    }
}
