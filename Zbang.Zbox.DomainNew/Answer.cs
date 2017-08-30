﻿using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class Answer
    {
        protected Answer()
        {

        }

        public Answer(Guid id, string text, Question question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }
            Id = id;
            Quiz = question.Quiz;
            if (string.IsNullOrWhiteSpace(text))
            {
                text = null;
            }
            text = text?.Trim();
            Text = text;
            Question = question;

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor Resharper
            DateTimeUser = new UserTimeDetails(Quiz.User.Id);
        }

        public virtual Guid Id { get; private set; }
        public virtual Quiz Quiz { get; private set; }
        public virtual string Text { get; private set; }
        public virtual Question Question { get; private set; }

        public virtual UserTimeDetails DateTimeUser { get; private set; }

        public virtual ICollection<SolvedQuestion> SolvedQuizAnswer { get; set; }


        public virtual void UpdateCorrectAnswer()
        {
            Question.UpdateCorrectAnswer(this);
        }


        public virtual void UpdateText(string newText)
        {
            //Throw.OnNull(newText, "newText", false);
            if (newText == null)
            {
                throw new ArgumentNullException(nameof(newText));
            }
            Text = newText.Trim();
        }
    }
}
