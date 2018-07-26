using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("Cloudents.Infrastructure.Data")]

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Question
    {
        public Question(QuestionSubject subject, string text, decimal price, int attachments, User user)
        : this()
        {
            Subject = subject;
            Text = text?.Trim();
            Price = price;
            Attachments = attachments;
            User = user;
            Updated = Created = DateTime.UtcNow;

            if (user.Fictive)
            {
                Updated = DateTimeHelpers.NextRandomDate(1);
            }

            QuestionCreateTransaction();
        }

        [UsedImplicitly]
        protected Question()
        {
            Answers = Answers ?? new List<Answer>();
        }

        public virtual long Id { get; protected set; }
        public virtual QuestionSubject Subject { get; protected set; }
        public virtual string Text { get; protected set; }
        public virtual decimal Price { get; protected set; }

        public virtual int Attachments { get; protected set; }

        public virtual User User { get; protected set; }

        public virtual DateTime Created { get; protected set; }

        public virtual Answer CorrectAnswer { get; protected set; }

        public virtual IList<Answer> Answers { get; protected set; }

        public virtual DateTime Updated { get; set; }

        protected internal virtual IList<Transaction> Transactions { get; set; }


        public virtual void QuestionCreateTransaction()
        {
            var t = Transaction.QuestionCreate(this);// new Transaction(ActionType.Question, TransactionType.Stake, -Price);
            User.AddTransaction(t);
        }

        public virtual void QuestionDeleteTransaction()
        {
            foreach (var transaction in Transactions)
            {
                transaction.Question = null;
            }
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

            //TODO remove from earned or question from user
            MarkCorrectTransaction(correctAnswer);
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





    }
}