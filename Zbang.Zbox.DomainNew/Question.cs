using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class Question
    {
        public static readonly string[] AllowedHtmlTag = { "ul", "ol", "li", "font", "img", "p", "b", "u", "i", "span", "strong", "em", "table", "tbody", "tr","td" };
        protected Question()
        {

        }

        public Question(Guid id, Quiz quiz, string text)
        {
            if (quiz == null) throw new ArgumentNullException(nameof(quiz));
            Id = id;
            if (string.IsNullOrWhiteSpace(text))
            {
                text = null;
            }
            Text = text?.Trim(); ;
            Quiz = quiz;
            DateTimeUser = new UserTimeDetails(quiz.User.Id);
        }

        public virtual Guid Id { get; private set; }
        public virtual Quiz Quiz { get; private set; }
        public virtual string Text { get; private set; }
        public virtual UserTimeDetails DateTimeUser { get; private set; }
        public virtual Answer RightAnswer { get; private set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public virtual ICollection<Answer> Answers { get; protected set; }

        public virtual void UpdateCorrectAnswer(Answer answer)
        {
            RightAnswer = answer;
        }

        public virtual void UpdateText(string newText)
        {
            if (string.IsNullOrEmpty(newText))
            {
                newText = null;
            }
            //Throw.OnNull(newText, "newText", false);
            newText = newText?.Trim();
            Text = newText;
            DateTimeUser.UpdateTime = DateTime.UtcNow;
        }
    }
}
