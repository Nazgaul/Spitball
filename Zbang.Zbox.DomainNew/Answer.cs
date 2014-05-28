using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Domain
{
    public class Answer
    {
        protected Answer()
        {

        }
        public Answer(Guid id, string text, Question question)
        {
            Throw.OnNull(text, "text", false);

            Id = id;
            Quiz = question.Quiz;
            if (string.IsNullOrWhiteSpace(text))
            {
                text = null;
            }
            if (text != null)
            {
                text = text.Trim();
            }
            Text = text;
            Question = question;
           
            DateTimeUser = new UserTimeDetails(Quiz.Owner.Email);
        }
        public virtual Guid Id { get; private set; }
        public virtual Quiz Quiz { get; private set; }
        public virtual string Text { get; private set; }
        public virtual Question Question { get; private set; }

        public virtual UserTimeDetails DateTimeUser { get; private set; }


        public virtual void UpdateCorrectAnswer()
        {
            Question.UpdateCorrectAnswer(this);
        }

        
        public virtual void UpdateText(string newText)
        {
            Throw.OnNull(newText, "newText", false);
            Text = newText.Trim();
        }
    }
}
