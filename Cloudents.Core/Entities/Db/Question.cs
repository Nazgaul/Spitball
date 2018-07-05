using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cloudents.Core.Enum;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Question
    {
        public Question(QuestionSubject subject, string text, decimal price, int attachments, User user)
        {
            Subject = subject;
            Text = text;
            Price = price;
            Attachments = attachments;
            User = user;
            Created = DateTime.UtcNow;

            QuestionCreateTransaction();
        }

        [UsedImplicitly]
        protected Question()
        {
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


        public virtual Transaction QuestionCreateTransaction()
        {
            var t = new Transaction(ActionType.Question, TransactionType.Stake, -Price);
            User.AddTransaction(t);
            return t;
        }

        public virtual void QuestionDeleteTransaction()
        {
            var t = new Transaction(ActionType.DeleteQuestion, TransactionType.Stake, Price);
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

        public virtual IEnumerable<Transaction> MarkCorrectTransaction(Answer correctAnswer)
        {
            //var list = new List<Transaction>();
            var questionUser = User;
            var t1 = new Transaction(ActionType.QuestionCorrect, TransactionType.Stake, Price);
            var t2 = new Transaction(ActionType.QuestionCorrect, TransactionType.Spent, -Price);
            questionUser.AddTransaction(t1);
            questionUser.AddTransaction(t2);

            var answerUser = correctAnswer.User;


            var tAnswer = new Transaction(ActionType.QuestionCorrect, TransactionType.Earned,
                Price);
            answerUser.AddTransaction(tAnswer);

            return new[] {t1, t2, tAnswer};
        }



    }
}