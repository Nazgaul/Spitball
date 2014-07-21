using System;

namespace Zbang.Zbox.Domain
{
    public class Discussion
    {
        protected Discussion()
        {

        }
        public Discussion(Guid id, User owner, string text, Question question)
        {
            if (owner == null) throw new ArgumentNullException("owner");
            if (text == null) throw new ArgumentNullException("text");
            if (question == null) throw new ArgumentNullException("question");

// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = id;
            Owner = owner;
            Text = text.Trim();
            CreationTime = DateTime.UtcNow;
            Quiz = question.Quiz;
            Question = question;
            Quiz.Box.UserTime.UpdateUserTime(owner.Email);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

        }
        public virtual Guid Id { get; set; }
        public virtual User Owner { get; set; }
        public virtual string Text { get; set; }
        public virtual DateTime CreationTime { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual Question Question { get; set; }
    }
}
