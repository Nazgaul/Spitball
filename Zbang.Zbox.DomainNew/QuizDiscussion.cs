using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class QuizDiscussion
    {
        protected QuizDiscussion()
        {

        }

        public QuizDiscussion(Guid id, User owner, string text, Question question)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (question == null) throw new ArgumentNullException(nameof(question));

// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = id;
            Owner = owner;
            Text = text.Trim();
            CreationTime = DateTime.UtcNow;
            Quiz = question.Quiz;
            Question = question;
            
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

        }

        public virtual Guid Id { get; set; }
        public virtual User Owner { get; set; }
        public virtual string Text { get; set; }
        public virtual DateTime CreationTime { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual Question Question { get; set; }

        protected virtual ICollection<Updates> Updates { get; set; }
    }
}
