using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class Question
    {
        protected Question()
        {

        }
        public Question(Guid id, Quiz quiz, string text)
        {
            if (quiz == null) throw new ArgumentNullException("quiz");
            Id = id;
            if (string.IsNullOrWhiteSpace(text))
            {
                text = null;
            }
            if (text != null)
            {
                text = text.Trim();
                
            }

            Text = text;
            Quiz = quiz;
            DateTimeUser = new UserTimeDetails(quiz.Owner.Email);
        }
        public virtual Guid Id { get; private set; }
        public virtual Quiz Quiz { get; private set; }
        public virtual string Text { get; private set; }
        public virtual UserTimeDetails DateTimeUser { get; private set; }
        public virtual Answer RightAnswer { get; private set; }
// ReSharper disable once UnusedAutoPropertyAccessor.Local
        public virtual ICollection<Answer> Answers { get; private set; }

        public virtual void UpdateCorrectAnswer(Answer answer)
        {
            RightAnswer = answer;
        }

        public virtual void UpdateText(string newText)
        {
            //Throw.OnNull(newText, "newText", false);
            if (newText != null)
            {
                newText = newText.Trim();
            }
            Text = newText;
            DateTimeUser.UpdateTime = DateTime.UtcNow;

        }
    }


}
