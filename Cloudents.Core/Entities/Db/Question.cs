using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            //User.AddTransaction(ActionType.Question, TransactionType.Stake, -price);
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

            ////TODO remove from earned or question from user
            MarkCorrectTransaction();
        }

        public virtual IEnumerable<Transaction> MarkCorrectTransaction()
        {
            var list = new List<Transaction>();
            var t1 = new Transaction(ActionType.QuestionCorrect, TransactionType.Spent, -Price);
            var t2 = new Transaction(ActionType.QuestionCorrect, TransactionType.Stake, Price);
            list.AddRange(new[] { t1, t2 });
            User.AddTransaction(t1);
            User.AddTransaction(t2);

            //User.AddTransaction(ActionType.QuestionCorrect, TransactionType.Spent, -Price);
            //User.AddTransaction(ActionType.QuestionCorrect, TransactionType.Stake, Price);

            foreach (var answer in Answers)
            {
                if (answer.Id == CorrectAnswer.Id)
                {
                    var t3 = new Transaction(ActionType.QuestionCorrect, TransactionType.Earned,
                        Price);
                    answer.User.AddTransaction(t3);

                    list.Add(t3);
                }

                var t4 = new Transaction(ActionType.QuestionCorrect, TransactionType.Pending, -Price);
                answer.User.AddTransaction(t4);
                list.Add(t4);
            }

            return list;
        }
    }
}