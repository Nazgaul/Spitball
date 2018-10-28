using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Question : IEvents
    {
        public Question(QuestionSubject subject, string text, decimal price, int attachments, User user, QuestionColor color)
        : this()
        {
            Subject = subject;
            Text = text?.Trim();
            Price = price;
            Attachments = attachments;
            User = user;
            Updated = Created = DateTime.UtcNow;
            if (color != QuestionColor.Default)
            {
                Color = color;
            }

            //if (user.Fictive)
            //{
            //    Updated = DateTimeHelpers.NextRandomDate(1);
            //}
            State = QuestionState.Pending;
            QuestionCreateTransaction();
            if (State.GetValueOrDefault() == QuestionState.Ok)
            {
                Events.Add(new QuestionCreatedEvent(this));
            }


        }

        [UsedImplicitly]
        protected Question()
        {
            Answers = Answers ?? new List<Answer>();
            Events = new List<IEvent>();
        }

        public virtual long Id { get; protected set; }
        public virtual QuestionSubject Subject { get; protected set; }
        public virtual string Text { get; protected set; }
        public virtual decimal Price { get; protected set; }

        public virtual int Attachments { get; protected set; }

        public virtual User User { get; protected set; }

        public virtual DateTime Created { get; protected set; }
        public virtual DateTime Updated { get; set; }

       [CanBeNull]
       public virtual Answer CorrectAnswer { get;  set; }

        public virtual IList<Answer> Answers { get; protected set; }


        protected internal virtual IList<Transaction> Transactions { get; set; }

        public virtual QuestionColor? Color { get; set; }
        public virtual QuestionState? State { get; set; }


        public virtual void QuestionCreateTransaction()
        {
            var t = Transaction.QuestionCreate(this);// new Transaction(ActionType.Question, TransactionType.Stake, -Price);
            User.AddTransaction(t);
        }

        public virtual void QuestionDeleteTransaction()
        {
            var t = Transaction.QuestionDelete(this);// new Transaction(ActionType.DeleteQuestion, TransactionType.Stake, Price);
            User.AddTransaction(t);
        }

        public virtual void MarkAnswerAsCorrect(Answer correctAnswer)
        {
            if (CorrectAnswer != null)
            {
                throw new InvalidOperationException("Already have correct answer");
            }
            CorrectAnswer = correctAnswer;

            MarkCorrectTransaction(correctAnswer);

            Events.Add(new MarkAsCorrectEvent(correctAnswer));
        }

        public virtual void MarkCorrectTransaction(Answer correctAnswer)
        {
            var questionUser = User;
            var t1 = Transaction.CorrectAnswer(TransactionType.Stake, this, correctAnswer); //new Transaction(ActionType.AnswerCorrect, TransactionType.Stake, Price);
            var t2 = Transaction.CorrectAnswer(TransactionType.Spent, this, correctAnswer);// new Transaction(ActionType.AnswerCorrect, TransactionType.Spent, -Price);
            questionUser.AddTransaction(t1);
            questionUser.AddTransaction(t2);

            var answerUser = correctAnswer.User;


            var tAnswer = Transaction.CorrectAnswer(TransactionType.Earned, this, correctAnswer);// new Transaction(ActionType.AnswerCorrect, TransactionType.Earned,
                                                                                                 //Price);
            answerUser.AddTransaction(tAnswer);

            //return new[] {t1, t2, tAnswer};
        }

        public virtual Answer AddAnswer(string text, int attachments, User user)
        {
            var answer = new Answer(this, text, attachments, user);
            Answers.Add(answer);
            return answer;
        }


        public virtual IList<IEvent> Events { get; protected set; }
    }
}