using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Domain
{
    public class Question
    {
        protected Question()
        {

        }
        public Question(Guid id, Quiz quiz, string text)
        {
            Id = id;
            Text = text;
            Quiz = quiz;
            DateTimeUser = new UserTimeDetails(quiz.Owner.Email);
        }
        public virtual Guid Id { get; private set; }
        public virtual Quiz Quiz { get; private set; }
        public virtual string Text { get; private set; }
        public virtual UserTimeDetails DateTimeUser { get; private set; }
        public virtual Answer RightAnswer { get; private set; }

        public virtual void UpdateCorrectAnswer(Answer answer)
        {
            RightAnswer = answer;
        }

        public void UpdateText(string newText)
        {
            Throw.OnNull(newText, "newText", false);
            Text = newText;
            DateTimeUser.UpdateTime = DateTime.UtcNow;

        }
    }


}
